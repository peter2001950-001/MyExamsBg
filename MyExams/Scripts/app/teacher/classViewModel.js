
function ClassViewModel() {
    var self = this;
    self.lastExams = ko.observable();
    self.tests = ko.observable();
    self.students = ko.observable();
    self.studentNo = ko.observable();
    self.studentFirstName = ko.observable();
    self.studentLastName = ko.observable();
    self.ifAlert = ko.observable(false);
    self.alertText = ko.observable();
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

    self.NewStudent = function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.ajax({
            type: "POST",
            url: "/t/AddStudent",
            data: {
                __RequestVerificationToken: token,
                no: self.studentNo(),
                firstName: self.studentFirstName(),
                lastName: self.studentLastName(),
                classCode: classCode
            },
            success: function (data) {
                switch (data.status) {
                    case "OK":
                        $("#addStudent").modal("hide");
                        self.GetStudents();
                        break;
                    case "ERR1":
                        self.alertText("За зададения номер има вече съществуващ ученик");
                        self.ifAlert(true);
                    case "ERR2":
                        self.alertText("Въведеният номер не е число");
                        self.ifAlert(true);
                        break;
                    case "ERR3":
                        location.reload();
                        break;
                    default:
                }
            }
        });
    }
    self.gotoUrl = function (item) {
        window.location.href = "/t/gTest?id=" + item.id;
    }
}
var vm = new ClassViewModel();
ko.applyBindings(vm);

vm.GetLastExams();
