function StudentClassViewModel() {
    var self = this;
    self.classes = ko.observable();

    self.pull = function () {
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/s/GetClasses",
            success: function (data) {
                if (data.status === "OK") {
                    self.classes(data.classes);
                 

                }
            }
        });
    }

    self.gotoUrl = function (item) {
        window.location.href = "/s/class?id=" + item.code;
    };
}
var vm = new StudentClassViewModel();
ko.applyBindings(vm);

$(document).ready(function () {
    vm.pull();
})
var joinClassRedirection = function (data) {
    if (data.status == "OK") {
        window.location.href = "/s/class?id=" + data.code;
    } else {
        alert("Възникна грешка при създавянето на нов клас!")
    }
}