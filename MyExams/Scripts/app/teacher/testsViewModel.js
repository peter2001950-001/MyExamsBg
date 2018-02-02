
function testViewModel() {
    self = this;
    self.tests = ko.observableArray();

    self.GetTests = function() {
            $.ajax({
                type: "GET",
                dataType: "json",
                contentType: "application/json",
                url: "/t/getTests",
                success: function (data) {
                    if (data.status === "OK") {
                        self.tests(data.tests);
                       
                    }
                }
            });
    }
    self.gotoUrl = function (item) {
        window.location.href = "/t/testDesign?id=" + item.testCode;
    }
}

var myVm = new testViewModel();
ko.applyBindings(myVm);

$(document).ready(function () {
    myVm.GetTests();
})
