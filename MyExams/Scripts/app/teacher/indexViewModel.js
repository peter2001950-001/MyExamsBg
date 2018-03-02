function indexViewModel(){
    var self = this;
    self.uploadSessionData = ko.observable();
    self.isQuestionsToConfirm = ko.observable(false);
    self.tests = ko.observable();
    self.classes = ko.observable();
    self.uploadSessionNotification = function () {
        $.ajax({
            type: "get",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/UploadSessionNotification",
            success: function (data) {
                if (data.status === "HAS") {
                    self.uploadSessionData(data);
                    $("#uploadSessionNotification").modal("show");
                }
            }
        });
    };
    self.syncIndex = function () {
        $.ajax({
            type: "get",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/syncIndex",
            success: function (data) {
                if (data.status === "OK") {
                    self.classes(data.classes);
                    self.tests(data.tests);
                    self.isQuestionsToConfirm(data.isQuestionsToBeChecked);
                }
            }
        });
    }
    self.gotoClassUrl = function (item) {
        window.location.href = "/t/class?id=" + item.code;
    };
    self.gotoTestUrl = function (item) {
        window.location.href = "/t/testDesign?id=" + item.testCode;
    };
};

var ivm = new indexViewModel();
ko.applyBindings(ivm);
$(document).ready(function () {
    ivm.uploadSessionNotification();
    $(function () {
        $('[data-toggle="popover"]').popover()
    })
    ivm.syncIndex();
})