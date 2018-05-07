function indexViewModel(){
    var self = this;
    var alfabet = "АБВГДЕЖЗИЙКЛМ";
    self.uploadSessionData = ko.observable();
    self.isQuestionsToConfirm = ko.observable(false);
    self.tests = ko.observable();
    self.classes = ko.observable();
    self.question = ko.observable();
    self.currentNo = ko.observable(1);
    self.count = ko.observable();
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
                    if (data.classes.length != 0) {
                        self.classes(data.classes);
                    }
                    if (data.tests.length != 0) {
                        self.tests(data.tests);
                    }
                    self.isQuestionsToConfirm(data.isQuestionsToBeChecked);
                    self.count(data.count);
                }
            }
        });
    }
    self.getQuestion = function () {
        $.ajax({
            type: "get",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/GetWrittenQuestion",
            success: function (data) {
                if (data.status === "OK") {
                    data.question.optionsArray = [];
                    if (data.question.type == "0") {
                        data.question.choiceText = "Изберете отговор:";
                  
                        for (var i = 0; i <= data.question.options; i++) {
                            if (i == 0) {
                                data.question.optionsArray.push("ДР.");
                            } else {
                                data.question.optionsArray.push(alfabet[i - 1]);
                                data.question.correctAnswer = "";
                            }
                        }
                    } else {
                        data.question.choiceText = "Изберете брой точки:";
                        for (var i = 0; i <= data.question.options; i++) {
                            
                            data.question.optionsArray.push(i);
                        }
                    }
                    self.question(data.question);

                    $("#questionsToBeChecked").modal("show");
                } else if (data.status == "ERR3") {

                    $("#questionsToBeChecked").modal("hide");
                    self.syncIndex();
                }
            }
        });
    }
    self.givePoints = function (points, parent) {
        $.ajax({
            type: "post",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/GivePoints",
            data: {
                questionId: self.question().id,
                option: points
            },
            success: function (data) {
                if (data.status === "OK") {
                    self.getQuestion();
                    self.currentNo(self.currentNo() + 1);
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