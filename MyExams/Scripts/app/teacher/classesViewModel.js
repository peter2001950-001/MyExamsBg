function ClassesViewModel() {
    
    var self = this;
    self.classes = ko.observable();


    self.pull = function () {
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/t/GetClasses",
            success: function (data) {
                if (data.status === "OK") {
                    self.classes(data.classes);
                    console.log(self.classes);

                }
            }
        });
    };
    self.gotoUrl = function (item) {
        window.location.href = "/t/class?id="+item.code;
    };
};



var myVm = new ClassesViewModel();
ko.applyBindings(myVm);

$(document).ready(function () {
    myVm.pull();
})

var createTestRedirection = function (data) {
    if (data.status == "OK") {
        window.location.href = "/t/class?id=" + data.code;
    } else {
        alert("Възникна грешка при създавянето на нов клас!")
    }
}