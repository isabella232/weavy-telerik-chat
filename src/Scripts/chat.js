var Chat = (function () {
    var _weavy = null;
    var _listView = null;
    var _chat = null;
    var _users = null;
    var _conversations = null;
    var _activeConversation = null;
    var _badge = null;

    function init() {
        _weavy = new Weavy({ jwt: getToken });

        _weavy.whenLoaded().then(function () {

            // init badge
            _weavy.ajax("api/conversations/unread").then(function (unread) {
                _badge = $("#badge").kendoButton({
                    badge: {
                        text: unread,
                        max: 99,
                        shape: "pill",
                        themeColor: "error",
                        visible: unread > 0 ? true : false
                    }
                });
            });

            // handle realtime events
            _weavy.connection.on("message-inserted.weavy", function (e, message) {
                if (message.conversation === _activeConversation && _weavy.user.id !== message.createdBy.id) {
                    renderMessage(message);
                } else {
                    $("#listView").data("kendoListView").dataSource.read();
                }
            });

            _weavy.connection.on("badge.weavy", function (e, item) {
                var badge = _badge.find(".k-badge")
                if (item.conversations > 0) {
                    badge.removeClass("k-hidden");
                } else {
                    badge.addClass("k-hidden");
                }
                badge.text(item.conversations);
            });

            _weavy.connection.on("typing.weavy", function (e, item) {
                if (item.conversation === _activeConversation) {
                    _chat.renderUserTypingIndicator({ name: item.name, id: item.user.id });
                }
            });

            _weavy.connection.on("typing-stopped.weavy", function (e, item) {
                if (item.conversation === _activeConversation) {
                    _chat.clearUserTypingIndicator({ name: item.name, id: item.user.id });
                }
            });

            // select users for new conversation
            $("#users").kendoMultiSelect({
                placeholder: "Select conversation members...",
                dataTextField: "profile.name",
                dataValueField: "id",
                autoBind: false,
                dataSource: {
                    serverFiltering: true,
                    transport: {
                        read: function (options) {
                            _weavy.ajax("/api/users").then(function (users) {
                                users.data.forEach(function (u) {
                                    if (typeof (u.profile.name) === "undefined") {
                                        u.profile.name = u.username;
                                    }
                                });
                                options.success(users.data);
                            });
                        }
                    }
                }
            });

            _users = $("#users").data("kendoMultiSelect")

            // load conversations
            _conversations = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        _weavy.ajax("/api/conversations").then(function (conversations) {

                            if (conversations.length === 0) {
                                $("#no-conversations").removeClass("d-none");
                            } else {
                                $("#no-conversations").addClass("d-none");

                                conversations.forEach(function (c) {
                                    c.thumb = _weavy.options.url + c.thumb.replace("{options}", "96");
                                    if (c.isRoom) {
                                        c.title = c.name;
                                    } else {
                                        if (c.members.length > 1) {
                                            var u = c.members.filter(x => x.id !== _weavy.user.id)[0];
                                            c.title = typeof (u.name) === "undefined" ? u.username : u.name;
                                        } else {
                                            c.title = c.members[0].name === "undefined" ? c.members[0].username : c.members[0].name;
                                        }
                                    }

                                });
                                options.success(conversations);

                                // open first conversation
                                if (_activeConversation == null) {
                                    loadConversation(conversations[0].id);
                                }
                            }
                        });
                    }
                }
            });

            _listView = $("#listView").kendoListView({
                dataSource: _conversations,
                selectable: "single",
                template: kendo.template($("#template").html())
            }).data("kendoListView");

            _listView.bind("change", onChangeConversation);
        });
    }

    // load chat and mark conversation as active
    function loadConversation(id) {
        loadChat(id);

        if (id != null) {
            _listView.select(_listView.element.find('[data-id="' + id + '"]'));
        }
    }

    // create new conversation
    $(document).on("click", "#create-conversation", function () {
        if (_users.value().length > 0) {
            _weavy.ajax("/api/conversations", {
                members: _users.value()
            }, "POST").then(function (response) {
                $("#listView").data("kendoListView").dataSource.read();
                loadConversation(response.id);
                _users.value([]);
                $("#new-conversation").modal("hide");
            });
        }
    });

    function onChangeConversation(e) {
        $(e.sender.select()).removeClass("unread");
        loadChat(e.sender.select()[0].dataset.id);
    }

    // load messages in chat
    function loadChat(id) {
        _weavy.ajax("/api/conversations/" + id).then(function (conversation) {

            _activeConversation = conversation.id;

            _weavy.ajax("/api/conversations/" + id + "/messages").then(function (messages) {

                if (_chat !== null) {
                    $("#chat").data("kendoChat").destroy();
                    $("#chat").empty();
                }

                $("#chat").kendoChat({
                    user: {
                        iconUrl: _weavy.options.url + _weavy.user.thumb.replace("{options}", "96"),
                        name: _weavy.user.name
                    }, sendMessage: function (e) {
                        _weavy.ajax("api/conversations/" + id + "/messages", {
                            text: e.text
                        }, "POST").then(function (message) {
                        });
                    }, typingStart: function (e) {
                        _weavy.ajax("api/conversations/" + id + "/typing", null, "POST");
                    }, typingEnd: function (e) {
                        _weavy.ajax("api/conversations/" + id + "/typing", null, "DELETE");
                    }
                });

                _chat = $("#chat").data("kendoChat");

                if (typeof (messages.data) !== "undefined") {
                    messages.data.forEach(function (m) {
                        renderMessage(m);
                    });
                }
            });
        });
    }

    // renders a message in the active chat
    function renderMessage(message) {
        var user = message.createdBy.id == _weavy.user.id ? _chat.getUser() : {
            id: message.createdBy.id,
            name: typeof (message.createdBy.name) === "undefined" ? message.createdBy.username : message.createdBy.name,
            iconUrl: _weavy.options.url + message.thumb.replace("{options}", "96"),
        };

        _chat.renderMessage({
            type: "text",
            text: message.text,
            timestamp: new Date(message.createdAt)
        }, user);
    }

    // call local api to get token for user 
    var getToken = function () {
        return new Promise(function (resolve, reject) {
            $.ajax({
                url: "/account/token",
                success: function (token) {
                    resolve(token);
                },
                error: function (result) {
                    reject("Failed to retrieve token");
                }
            });
        });
    }

    return {
        init: init
    };
})();
