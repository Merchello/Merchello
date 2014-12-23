'use strict';

/* jasmine specs for models go here */

describe("Notifications Models", function () {
    describe("Notificaiton Method Model", function () {
        var notificationMethod;
        var nMethodfs;

        beforeEach(function () {
            var nMessagefs = {
                key: "123",
                name: "Go home",
                description: "a small message of unknown descent",
                replyTo: "The world",
                bodyText: "123",
                maxLength: "222",
                bodyTextIsFilePath: "maybe",
                monitorKey: "3F4Ga43eS",
                methodKey: "PCS",
                recipients: "me, you, anyone else",
                disabled: "ehh"
            }

            nMethodfs = {
                key: "2234",
                name: "carrier pidgeon",
                providerKey: "onedirection4ever",
                description: "attach notification to pideon named Gleeson.",
                serviceCode: "here birdie birdie birdie",
                notificationMessages: [nMessagefs]
            }

            notificationMethod = new merchello.Models.NotificationMethod(nMethodfs);
        });

        it("should create a populated notification method", function () {
            expect(notificationMethod).toBeDefined();
            for (var i in nMethodfs) {
                if (typeof (nMethodfs[i]) != "object")
                    expect(notificationMethod[i]).toBe(nMethodfs[i]);
            }
            
            expect(notificationMethod["notificationMessages"].length).toBe(1);
            expect(notificationMethod["notificationMessages"][0].key).toBe("123");
        });

        it("should create an empty notification method", function () {
            notificationMethod = new merchello.Models.NotificationMethod();
            expect(notificationMethod).toBeDefined();
            for (var i in nMethodfs) {
                if (typeof (nMethodfs[i]) != "object")
                    expect(notificationMethod[i]).toBe("");
            }

            expect(notificationMethod["notificationMessages"].length).toBe(0);
        });

        it("should display the editor", function() {
            var dev = {editorView: "Well this is arbitrary"};
            notificationMethod.dialogEditorView = dev;
            expect(notificationMethod.displayEditor()).toBe(dev.editorView);
        });
    });

    describe("Notification Message Model", function () {
        var notificationMessage;
        var nMessagefs;

        beforeEach(function () {
            nMessagefs = {
                key: "123",
                name: "Go home",
                description: "a small message of unknown descent",
                replyTo: "The world",
                bodyText: "123",
                maxLength: "222",
                bodyTextIsFilePath: "maybe",
                monitorKey: "3F4Ga43eS",
                methodKey: "PCS",
                recipients: "me, you, anyone else",
                disabled: "ehh"
            }
            notificationMessage = new merchello.Models.NotificationMessage(nMessagefs);
        });

        it("should create an empty notification message", function () {
            notificationMessage = new merchello.Models.NotificationMessage();
            expect(notificationMessage).toBeDefined();
            for (var i in nMessagefs) {
                expect(notificationMessage[i]).toBe("");
            }
        });

        it("should create a filled out notification message", function () {
            expect(notificationMessage).toBeDefined();
            for (var i in nMessagefs) {
                expect(notificationMessage[i]).toBe(nMessagefs[i]);
            }
        });
    });

    describe("Notification Subscriber Model", function () {
        var notificationSubscriber;
        var nsfs;

        beforeEach(function () {
            nsfs = {
                pk: "asdasdasd",
                email: "wut@example.com"
            }

            notificationSubscriber = new merchello.Models.NotificationSubscriber(nsfs);
        });

        it("should create an empty Notification Subscriber", function () {
            notificationSubscriber = new merchello.Models.NotificationSubscriber();
            expect(notificationSubscriber).toBeDefined();
            for (var i in nsfs) {
                expect(notificationSubscriber[i]).toBe("");
            }
        });

        it("should create a filled out Notification Subscriber", function () {
            expect(notificationSubscriber).toBeDefined();
            for (var i in nsfs) {
                expect(notificationSubscriber[i]).toBe(nsfs[i]);
            }
        });
    })
});