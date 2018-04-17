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
    self.placeholder = ko.computed(function () {
        return "Отговор " + self.optionNum();
    }, self)
    self.CheckOption = function () {
      
        if (self.checkLabel() == "done") {
           
                self.checkLabel("check_circle");
                self.checkedClass("option-checked");
                self.isChecked(true);
            } else {
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
    var firstTime = true;
    self.addOption = function (optionText, optionNum) {
        if (!firstTime) {

        var option = new QuestionOption(optionText, optionNum, self.options().length, self.questionId, self.sectionId);
        self.options.push(option);
        } else {
            firstTime = false;
        }
    }.bind(self);
    self.type = ko.observable(type); // 0 - choice, 1 - text
    self.answerSizeOptions = ko.observableArray(["Кратък", "Среден", "Дълъг"]);
    self.selectedAnswerSize = ko.observable();
    self.correctAnswer = ko.observable();
    self.mixupOptions = ko.observable(true);
    self.points = ko.observable("");
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
    self.mixupOptions.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    })
    self.points.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onQuestionUpdate(self.sectionId, self.questionId);
        }
    });
    self.options.subscribe(function (item) {
        if (subscribeActivate) {
            $.ajax({
                type: "post",
                datatype: "json",
                contenttype: "application/json",
                url: "/t/elementupdate",
                data: {
                    action: item[0].status,
                    index: item[0].index,
                    type: "option",
                    sectionId: self.sectionId,
                    questionId: self.questionId,
                    testUniqueCode: testUniqueCode
                },
                success: function (data) {
                    if (data.status === "ok") {
                        self.tests(data.tests);
                        self.students(data.students);
                    }
                }
            });
        }
        
              
    }, self, "arrayChange");
   
  
}


function Section(sectionText, sectionId, mixupQuestions, questionsToShow, isQuestionsToShowSet, image) {
    var self = this;
    self.isQuestionsToShowSet = isQuestionsToShowSet;
    self.sectionText = ko.observable(sectionText);
    self.sectionId = sectionId;
    self.questions = ko.observableArray();
    self.focus = true;
    self.mixupQuestions = ko.observable(mixupQuestions);
    self.questionsToShow = ko.observable(questionsToShow);
    self.isAlert = ko.observable(false);
    self.alert = ko.observableArray();
    self.image = ko.observable(image);
    self.addQuestion = function (question) {
        self.questions.push(question);
        if (subscribeActivate) {
            if (!isQuestionsToShowSet) self.questionsToShow(self.questionsToShow() + 1);
        }
    }.bind(this);
    self.uploadFiles = function (file, sectionId) {
        console.log(sectionId);
            if (window.FormData !== undefined) {
                var data = new FormData();
                data.append("file" + 0, file);
                $.ajax({
                    type: "POST",
                    url: '/t/PictureUpload?testUniqueCode=' + testUniqueCode + "&sectionId=" + sectionId,
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        if (result.status == "OK") {
                            alert("Успешно качен");
                            self.image(result.image);
                        };
                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            }
        
    }
    self.questions.subscribe(function (item) {
        if (subscribeActivate) {
            $.ajax({
                type: "post",
                datatype: "json",
                contenttype: "application/json",
                url: "/t/elementupdate",
                data: {
                    action: item[0].status,
                    index: item[0].index,
                    type: "question",
                    sectionId: self.sectionId,
                    questionType: item[0].value.type(),
                    testUniqueCode: testUniqueCode
                },
                success: function (data) {
                    if (data.status === "OK") {
                        console.log(data);
                    }
                }
            });
        }
       
    }, self, "arrayChange");
    self.mixupQuestions.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onSectionUpdate(self.sectionId);
        }
    })
    self.sectionText.subscribe(function (item) {
        if (subscribeActivate) {
            testDesignViewModel.onSectionUpdate(self.sectionId);
        }
        });
    self.questionsToShow.subscribe(function (item) {
        if (subscribeActivate) {
            self.isQuestionsToShowSet = true;
            testDesignViewModel.onSectionUpdate(self.sectionId);
        }
    });
}

function TestDesignViewModel() {
    var stopDelete = true;
    var self = this;
    var bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";

    self.addBtnVisiblity = ko.observable(true);
    self.sections = ko.observableArray();
    self.testName = ko.observable();
    self.classes = ko.observable();
    self.showLoading = ko.observable(false);

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
                    var questionSizeOptions = ["Кратък", "Среден", "Дълъг"];
                    self.testName(data.testTitle);
                    for (var i in data.sections) {
                        var questionsToShow = data.sections[i].questionsToShow;
                        var areSet = true;
                        if (data.sections[i].questionsToShow == 0) {
                            questionsToShow = Object.keys(data.sections[i].questions).length;
                            console.log(Object.keys(data.sections[i].questions).length);
                            areSet = false;
                        }
                        self.sections.push(new Section(data.sections[i].text, data.sections[i].id, data.sections[i].mixupQuestions, questionsToShow, areSet, data.sections[i].image));
                        for (var p in data.sections[i].questions) {

                            self.sections()[i].addQuestion(new Question(data.sections[i].questions[p].text, data.sections[i].questions[p].id, false, data.sections[i].questions[p].type, data.sections[i].id));
                            if (data.sections[i].questions[p].type == "0") {

                                for (var q in data.sections[i].questions[p].options) {

                                    
                                    self.sections()[i].questions()[p].addOption(data.sections[i].questions[p].options[q].text, bgAlphabet[data.sections[i].questions[p].options[q].id]);

                                    if (data.sections[i].questions[p].options[q].isCorrect == true) {

                                        
                                        self.sections()[i].questions()[p].options()[q].isChecked(true);
                                        self.sections()[i].questions()[p].options()[q].checkLabel("check_circle");
                                        self.sections()[i].questions()[p].options()[q].checkedClass("option-checked");
                                        self.sections()[i].questions()[p].options()[q].isCheckOptionVisible(true);
                                    }
                                }
                                self.sections()[i].questions()[p].mixupOptions(data.sections[i].questions[p].mixupOptions);
                            } else if (data.sections[i].questions[p].type == "1") {

                                self.sections()[i].questions()[p].selectedAnswerSize(questionSizeOptions[data.sections[i].questions[p].answerSize]);
                                self.sections()[i].questions()[p].correctAnswer(data.sections[i].questions[p].correctAnswer);
                            }
                            self.sections()[i].questions()[p].points(data.sections[i].questions[p].points);
                        }
                    }
                    subscribeActivate = true;
                    document.body.scrollTop = 0;
                    document.documentElement.scrollTop = 0;
                }
            }
        });
    }
    self.GetResults = function () {

    }
    self.ShowPrintMenu = function () {
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/t/GetClasses",
            success: function (data) {
                if (data.status === "OK") {
                    for (var i in data.classes) {
                        data.classes[i].isChecked = false;
                        var studentLabel = (data.classes[i].studentsCount != 1) ? "ученици" : "ученик";
                        data.classes[i].label = data.classes[i].name + " (" + data.classes[i].subject + ") - " + data.classes[i].studentsCount + " " + studentLabel;
                        
                    }
                    self.classes(data.classes);
                }
            }
        });
    }

    self.ChooseClasses = function () {
        self.showLoading(true);
        var chosenClasses = [];
        for (var i in self.classes()) {
            if (self.classes()[i].isChecked == true) {
                chosenClasses.push(self.classes()[i].code);


            }
        }

        $.ajax({
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                testUniqueCode: testUniqueCode,
                chosenClasses: JSON.stringify(chosenClasses)
            }),
            url: "/t/ChooseClassesForTest",
            success: function (data) {
                if (data.status === "OK") {
                    window.location = getUrl + "?path=" + data.fName;
                }
                self.showLoading(false);
            }
        });
    }

    self.onQuestionUpdate = function (sectionId, questionId) {
        var question = self.sections()[sectionId].questions()[questionId];
        var questionObj = { id: questionId, text:  question.questionText()};
        if (question.type() == "0") {
            questionObj.options = [];
            for (var i in question.options()) {
                questionObj.options.push({ id: question.options()[i].id, text: question.options()[i].optionText(), isCorrect: question.options()[i].isChecked() });
            }
            questionObj.mixupOptions = question.mixupOptions();
        } else if (question.type() == "1") {
            questionObj.correctAnswer = question.correctAnswer();
            questionObj.selectedAnswerSize = question.selectedAnswerSize();

        }
       
        if(parseInt(question.points())){
            questionObj.points = parseInt(question.points());
        } else {
            question.points("");
        }
        
        $.ajax({
            type: "post",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/questionupdate",
            data: {
             data:  JSON.stringify({
                    testCode: testUniqueCode,
                    sectionId: sectionId,
                    question: questionObj
                })
            },
            success: function (data) {
                if (data.status === "OK") {
                    console.log(data);
                }
            }
        });
    }
    self.onSectionUpdate = function (sectionId) {
        if (parseInt(self.sections()[sectionId].questionsToShow()) == NaN) {
            self.sections()[sectionId].isAlert(true);
            self.sections()[sectionId].alert("В полето 'Брой въпроси' се въвежда число!");
        } else {
            if (parseInt(self.sections()[sectionId].questionsToShow()) > self.sections()[sectionId].questions().length) {
                self.sections()[sectionId].isAlert(true);
                self.sections()[sectionId].alert("В полето 'Брой въпроси' стойността трябва да е не по-голяма от " + self.sections()[sectionId].questions().length);
            } else {
                self.sections()[sectionId].isAlert(false);
            }
            var questionsToShow = self.sections()[sectionId].isQuestionsToShowSet ? parseInt(self.sections()[sectionId].questionsToShow()) : 0;
            $.ajax({
                type: "post",
                datatype: "json",
                contenttype: "application/json",
                url: "/t/sectionupdate",
                data: {
                    testUniqueCode: testUniqueCode,
                    index: sectionId,
                    name: self.sections()[sectionId].sectionText(),
                    mixupQuestions: self.sections()[sectionId].mixupQuestions(),
                    questionsToShow: questionsToShow
                },
                success: function (data) {
                    if (data.status === "OK") {
                        console.log(data);
                    }
                }
            });
        }
    }
  
    self.testName.subscribe(function (item) {
        $.ajax({
            type: "post",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/testnameupdate",
            data: {
                
                testUniqueCode: testUniqueCode,
                name: self.testName()
            },
            success: function (data) {
                if (data.status === "OK") {
                    console.log(data);
                }
            }
        });
    })
    


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
        
    }
    self.AddChoiceQuestion = function (item) {
      
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
        self.sections.push(new Section("", self.sections().length, true, 0, false, ""));
      
    }
    self.AddAnswer = function (item, parentId) {

        if (self.sections()[parentId].questions()[item.questionId].options().length < 8) {
            self.sections()[parentId].questions()[item.questionId].addOption("", getLetter(item, parentId));
        } else {
            alert("Вие достигнахте максималния брой отговори за този въпрос!");
        }
        stopDelete = true;
    }
    self.CheckOption = function (item, parentId, section) {
        if (item != undefined && parentId != undefined) {
            self.sections()[section.sectionId].questions()[parentId].CheckOption(item.id);
        }
       
        
    }
   
    self.DeleteOption = function (item, parentId, section) {
        
        if (! self.sections()[section.sectionId].questions()[parentId].options()[item.id].firstTime)
        {
             self.sections()[section.sectionId].questions()[parentId].options.remove( self.sections()[section.sectionId].questions()[parentId].options()[item.id]);
            for (var i = item.id; i <  self.sections()[section.sectionId].questions()[parentId].options().length; i++) {
                
                 self.sections()[section.sectionId].questions()[parentId].options()[i].id -= 1;
                 self.sections()[section.sectionId].questions()[parentId].options()[i].optionNum(bgAlphabet[ self.sections()[section.sectionId].questions()[parentId].options()[i].id]);
               
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
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
})
ko.bindingHandlers.selected = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var selected = ko.utils.unwrapObservable(valueAccessor());
        if (selected) element.select();
    }
};
ko.bindingHandlers.placeholder = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var underlyingObservable = valueAccessor();
        ko.applyBindingsToNode(element, { attr: { placeholder: underlyingObservable } });
    }
};