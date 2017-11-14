
function ClassViewModel() {
    var self = this;
    self.lastExams = ko.observable();
    self.tests = ko.observable();
    self.students = ko.observable();


    self.GetStudents = function () {
       applyDesign("marks");
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/t/GetStudents?uniqueCode=" + classCode,
            data: {
                uniqueCode: classCode
            },
            success: function (data) {
                if (data.status === "OK") {
                    self.tests(data.tests);
                    self.students(data.students);
                }
            }
       });

        console.log(classCode);
     
    }
    self.GetLastExams = function () {
       applyDesign("lastExams")
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/t/GetStudents?uniqueCode=" + classCode,
            success: function (data) {
                if (data.status === "OK") {
                   //TODO

                }
            }
       });
    }
}
var vm = new ClassViewModel();
ko.applyBindings(vm);

vm.GetLastExams();
