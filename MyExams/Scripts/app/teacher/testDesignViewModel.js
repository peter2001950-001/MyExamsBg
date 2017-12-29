var subscribeActivate = false;
function QuestionOption(optionText, optionNum, id, questionId, sectionId) {
    var self = this;
    self.optionText = ko.observable(optionText);
    self.optionNum = ko.observable(optionNum);
    self.focus = true;
    self.checkedClass = ko.observable("");
    self.checkLabel = ko.observable("");
    self.id = id;
    self.questionId = questionId;
    self.sectionId = sectionId;
    self.isChecked = ko.observable(false);
    self.isCheckOptionVisible = ko.observable(false)
    self.firstTime = true;
    self.CheckOption = function () {
      
        if (self.checkLabel() == "done") {
           
                self.checkLabel("check_circle");
                self.checkedClass("option-checked");
                self.isChecked(true);
            } else {
            console.log(self.id + " check_circle");
                self.checkLabel("done");
                self.checkedClass("");
                self.isChecked(false);
            }
    };
    self.CheckOptionShow = function (item, parentId) {
        
        self.isCheckOptionVisible(true);
    }
    self.CheckOptionHide = function (item, parentId) {
       
        if (self.isChecked() == false) {
            self.isCheckOptionVisible(false);
        }
      
    }
    self.optionText.subscribe(function (item) {
        if (subscribeActivate) {
              testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    });
    self.isChecked.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
       

    });
}


function Question(questionText, questionId, focus, type, sectionId) {
    var self = this;
    self.questionText = ko.observable(questionText);
    self.options = ko.observableArray();
    self.questionId = questionId;
    self.sectionId = sectionId;
    self.focus = focus;
    self.addOption = function (optionText, optionNum) {
        var option = new QuestionOption(optionText, optionNum, self.options().length, self.questionId, self.sectionId);
        self.options.push(option);
    }.bind(self);
    self.type = ko.observable(type); // 0 - choice, 1 - text
    self.answerSizeOptions = ko.observableArray(["Кратък", "Среден", "Дълъг"]);
    self.selectedAnswerSize = ko.observable();
    self.correctAnswer = ko.observable();
    self.inputCss = ko.computed(function () {

        switch (self.selectedAnswerSize()) {
            case "Кратък":
                return "col-4";
            case "Среден":
                return "";
            case "Дълъг":
                return "textarea";

        }
    }, self);
    self.inputVisible = ko.computed(function () {
        if (self.selectedAnswerSize() == undefined) {
            return false;
        } else {
            return true;
        }
    }, self);
    self.correctAnswer = ko.observable();
    self.CheckOption = function (optionId) {
        self.options()[optionId].CheckOption();

    }.bind(self);
    self.questionText.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    });
    self.correctAnswer.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    });
    self.selectedAnswerSize.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    });
    self.options.subscribe(function (item) {
        if (subscribeActivate) {

        }
        //$.ajax({
        //    type: "post",
        //    datatype: "json",
        //    contenttype: "application/json",
        //    url: "/t/elementupdate",
        //    data: {
               
        //    },
        //    success: function (data) {
        //        if (data.status === "ok") {
        //            self.tests(data.tests);
        //            self.students(data.students);
        //        }
        //    }
        //});
              
    }, self, "arrayChange");
   
  
}


function Section(sectionText, sectionId) {
    var self = this;
    self.sectionText = ko.observable(sectionText);
    self.sectionId = sectionId;
    self.questions = ko.observableArray();
    self.focus = true;
    self.addQuestion = function (question) {
        self.questions.push(question);
    }.bind(this);
    self.questions.subscribe(function (item) {
        //if (subscribeActivate) {
        //    $.ajax({
        //        type: "post",
        //        datatype: "json",
        //        contenttype: "application/json",
        //        url: "/t/elementupdate",
        //        data: {
        //            action: item[0].status,
        //            index: item[0].index,
        //            type: "question",
        //            sectionId: self.sectionId(),
        //            questionType: item[0],
        //            testUniqueCode: testUniqueCode
        //        },
        //        success: function (data) {
        //            if (data.status === "OK") {
        //                console.log(data);
        //            }
        //        }
        //    });
        //}
        console.log(item);
    }, self, "arrayChange");
}

function TestDesignViewModel() {
    var stopDelete = true;
    var self = this;
    var bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
    self.GetTest = function () {
        $.ajax({
            type: "post",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/gettest",
            data: {
                testUniqueCode: testUniqueCode
            },
            success: function (data) {
                if (data.status === "OK") {
                    var questionSizeOptions = ["Кратък", "Среден", "Дълъг"]
                    for (var i in data.sections) {
                        self.sections.push(new Section(data.sections[i].text, data.sections[i].id));
                        console.log(new Section(data.sections[i].text, Number(data.sections[i].id)))
                        for (var p in data.sections[i].questions) {
                            self.sections()[i].AddQuestion(new Question(data.sections[i].questions[p].text, data.sections[i].questions[p].id, false, data.sections[i].questions[p].type, data.sections[i].id));
                            if (data.sections[i].questions[p].type == "0") {
                                for (var q in data.sections[i].questions[p].options) {
                                    self.sections()[i].questions()[p].addOption(new QuestionOption(data.sections[i].questions[p].options[q].text, bgAlphabet[data.sections[i].questions[p].options[q].i], data.sections[i].questions[p].options[q].id, data.sections[i].questions[p].id, data.sections[i].id));
                                }
                            } else if (data.sections[i].questions[p].type == "1") {
                                self.sections()[i].questions()[p].selectedAnswerSize(questionSizeOptions[data.sections[i].questions[p].answerSize]);
                                self.sections()[i].questions()[p].correctAnswer(data.sections[i].questions[p].correctAnswer);
                            }
                        }
                    }
                    subscribeActivate = true;
                }
            }
        });
    }
    self.GetResults = function () {

    }
    self.onQuestionUpdate = function (sectionId, questionId) {
        console.log("section: " + sectionId + ", question: " + questionId);
    }
    self.testName = ko.observable();
    self.addBtnVisiblity = ko.observable(true);

    self.sections = ko.observableArray();


    self.sections.subscribe(function (changes) {
        if (subscribeActivate) {
            $.ajax({
                type: "post",
                datatype: "json",
                contenttype: "application/json",
                url: "/t/elementupdate",
                data: {
                    action: changes[0].status,
                    index: changes[0].index,
                    type: "section",
                    testUniqueCode: testUniqueCode
                },
                success: function (data) {
                    if (data.status === "OK") {
                        console.log(data);
                    }
                }
            });
        console.log(changes);
        }
    }, null, "arrayChange")
    self.AddQuestion = function (item)
    {
        self.addBtnVisiblity(false);
        console.log(item);
    }
    self.AddChoiceQuestion = function (item) {
        console.log(item);
        var question = new Question("", self.sections()[item.sectionId].questions().length, true, 0, item.sectionId);
        self.sections()[item.sectionId].addQuestion(question);
        self.addBtnVisiblity(true);
        
    }
    self.AddTextQuestion = function (item) {
        var question = new Question("", self.sections()[item.sectionId].questions().length, true, 1, item.sectionId);
        self.sections()[item.sectionId].addQuestion(question);
        self.addBtnVisiblity(true);
    }
    self.AddSection = function () {
        self.sections.push(new Section("", self.sections().length));
        console.log(new Section("", self.sections().length));
    }
    self.AddAnswer = function (item, parentId) {
        console.log(item.questionId);
        self.sections()[parentId].questions()[item.questionId].addOption("Отговор " + getLetter(item, parentId), getLetter(item, parentId));
        stopDelete = true;
    }
    self.CheckOption = function (item, parentId, section) {
        if (item != undefined && parentId != undefined) {
            self.sections()[section.sectionId].questions()[parentId].CheckOption(item.id);
        }
       
        
    }
   
    self.DeleteOption = function (item, parentId, section) {
        console.log(stopDelete);
        if (! self.sections()[section.sectionId].questions()[parentId].options()[item.id].firstTime)
        {
            console.log(item);
             self.sections()[section.sectionId].questions()[parentId].options.remove( self.sections()[section.sectionId].questions()[parentId].options()[item.id]);
            for (var i = item.id; i <  self.sections()[section.sectionId].questions()[parentId].options().length; i++) {
                
                 self.sections()[section.sectionId].questions()[parentId].options()[i].id -= 1;
                 self.sections()[section.sectionId].questions()[parentId].options()[i].optionNum(bgAlphabet[ self.sections()[section.sectionId].questions()[parentId].options()[i].id]);
                console.log( self.sections()[section.sectionId].questions()[parentId].options()[i]);
            }
        } else {
             self.sections()[section.sectionId].questions()[parentId].options()[item.id].firstTime = false;
        }
        
    }

    
    function getLetter(item, parentId) {

        return bgAlphabet[self.sections()[parentId].questions()[item.questionId].options().length];
    }
}

var testDesignViewModel = new TestDesignViewModel();
ko.applyBindings(testDesignViewModel);

$(document).ready(function () {
    testDesignViewModel.GetTest();
})
ko.bindingHandlers.selected = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var selected = ko.utils.unwrapObservable(valueAccessor());
        if (selected) element.select();
    }
};