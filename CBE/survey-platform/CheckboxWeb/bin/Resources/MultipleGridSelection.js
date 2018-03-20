var grid;
var SelectedIndexesPerPage = {};
var inputName;
function UpdateSelectedRows(pageIndex) {
    var selRows = grid.get_masterTableView().get_selectedItems();
    var result = "";
    if (typeof (selRows[0]) != "undefined") {
        result = selRows[0]._itemIndexHierarchical;
    }
    var i = 1;
    while (typeof (selRows[i]) != "undefined") {
        result = result + "," + selRows[i]._itemIndexHierarchical;
        i = i + 1;
    }
    SelectedIndexesPerPage[pageIndex] = result;
    SaveHashValues();
}

function RowSelected(sender, eventArgs) {
    var pageIndex = grid.get_masterTableView().get_currentPageIndex().toString();
    UpdateSelectedRows(pageIndex);
}
function RowDeselected(sender, eventArgs) {
    var pageIndex = grid.get_masterTableView().get_currentPageIndex().toString();
    UpdateSelectedRows(pageIndex);
}
function GridCreated(sender, eventArgs) {
    grid = sender;    
    var items = grid.get_masterTableView().get_dataItems();
}

function RestoreHashValues(InputName) {
    inputName = InputName;
    var saveInput = document.getElementById(inputName);
    var data = saveInput.value;
    if (data != "") {
        var rowsData = data.split(";");
        var i = 0;
        while (typeof (rowsData[i]) != "undefined") {
            var rowData = rowsData[i].split(":");
            SelectedIndexesPerPage[rowData[0]] = rowData[1];
            i = i + 1;
        }
    }
    
    var pageIndex = grid.get_masterTableView().get_currentPageIndex().toString();
    if (typeof (SelectedIndexesPerPage[pageIndex]) != "undefined") {
        var i = 0;
        var pageSelRows = SelectedIndexesPerPage[pageIndex].split(",");

        while (pageSelRows[i]) {
            var row = grid.get_masterTableView().get_dataItems()[pageSelRows[i]];
            grid.get_masterTableView().selectItem(row.get_element());
            i = i + 1;
        }
    }
}

function SaveHashValues() {
    var saveInput = document.getElementById(inputName);
    var i;
    var result = "";
    for (i = 0; i < grid.get_masterTableView().get_pageCount(); i++) {
        if (typeof (SelectedIndexesPerPage[i]) != "undefined") {
            if (result == "") {
                result = i + ":" + SelectedIndexesPerPage[i];
            }
            else {
                result = result + ";" + i + ":" + SelectedIndexesPerPage[i];
            }
        }
    }
    saveInput.value = result;
}