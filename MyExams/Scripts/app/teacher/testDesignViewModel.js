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

function TestDesignViewModel() {
    var stopDelete = true;
    var self = this;
    var bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
    self.GetQuestions = function () {

    }
    self.GetResults = function () {

    }
    self.testName = ko.observable();

    self.questions = ko.observableArray();
    self.AddQuestion = function () {
        var question = new Question("", self.questions().length, true);
        self.questions.push(question);
        console.log("pushed");
    }
    self.AddAnswer = function (item) {
        console.log("addAnswer");
        self.questions()[item.questionId].addOption("Отговор " + getLetter(item), getLetter(item));
        stopDelete = true;
    }
    self.CheckOption = function (item, parentId) {
        if (item != undefined && parentId != undefined) {
            self.questions()[parentId].CheckOption(item.id);
        }
       
        
    }
   
    self.DeleteOption = function (item, parentId) {
        console.log(stopDelete);
        if (!self.questions()[parentId].options()[item.id].firstTime)
        {
            console.log(item);
            self.questions()[parentId].options.remove(self.questions()[parentId].options()[item.id]);
            for (var i = item.id; i < self.questions()[parentId].options().length; i++) {
                
                self.questions()[parentId].options()[i].id -= 1;
                self.questions()[parentId].options()[i].optionNum(bgAlphabet[self.questions()[parentId].options()[i].id]);
                console.log(self.questions()[parentId].options()[i]);
            }
        } else {
            self.questions()[parentId].options()[item.id].firstTime = false;
        }
        
    }
  
    function getLetter(item) {
       
        return bgAlphabet[self.questions()[item.questionId].options().length];
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