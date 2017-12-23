function QuestionOption(optionText, optionNum, id) {
    this.optionText = ko.observable(optionText);
    this.optionNum = ko.observable(optionNum);
    this.focus = true;
    this.checkedClass = ko.observable("");
    this.checkLabel = ko.observable("");
    this.id = id;
    this.isChecked = ko.observable(false);
    this.isCheckOptionVisible = ko.observable(false)
    this.firstTime = true;
    this.CheckOption = function () {
      
        if (this.checkLabel() == "done") {
            console.log(this.id +  " done");
                this.checkLabel("check_circle");
                this.checkedClass("option-checked");
                this.isChecked(true);
            } else {
            console.log(this.id + " check_circle");
                this.checkLabel("done");
                this.checkedClass("");
                this.isChecked(false);
            }
    };
    this.CheckOptionShow = function (item, parentId) {
        console.log(item.id + " show");
        this.isCheckOptionVisible(true);
    }
    this.CheckOptionHide = function (item, parentId) {
        console.log(item.id + " hide");
        if (this.isChecked() == false) {
            this.isCheckOptionVisible(false);
        }
      
    }
}


function Question(questionText, questionId, focus) {
    
    this.questionText = questionText;
    this.options = ko.observableArray();
    this.questionId = questionId;
    this.focus = focus;
    this.addOption = function (optionText, optionNum) {
        var option = new QuestionOption(optionText, optionNum, this.options().length);
        this.options.push(option);
    }.bind(this);
    
    this.CheckOption = function (optionId) {
        this.options()[optionId].CheckOption();

    }.bind(this);
    
}

function Section(sectionText, sectionId) {
    this.sectionText = sectionText;
    this.sectionId = sectionId;
    this.questions = ko.observableArray();
    this.focus = true;
    this.addQuestion = function (question) {
        this.questions.push(question);
    }.bind(this);
}

function TestDesignViewModel() {
    var stopDelete = true;
    var self = this;
    var bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
    self.GetQuestions = function () {

    }
    self.GetResults = function () {

    }
    self.testName = ko.observable();

    self.sections = ko.observableArray();
    self.AddQuestion = function (item) {
        var question = new Question("", self.sections()[item.sectionId].questions().length, true);
        self.sections()[item.sectionId].addQuestion(question);
        console.log(item);
    }
    self.AddSection = function () {
        self.sections.push(new Section("", self.sections().length));
        console.log("sectionAdded");
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

ko.bindingHandlers.selected = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var selected = ko.utils.unwrapObservable(valueAccessor());
        if (selected) element.select();
    }
};