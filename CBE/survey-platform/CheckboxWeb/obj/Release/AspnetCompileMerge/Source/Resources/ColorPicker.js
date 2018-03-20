function HandleColorChange(sender, eventArgs) {
    $get("_customColor").value = sender.get_selectedColor();
}

function GetColorPicker() {
    return $find(id);
}

function UpdateColor(id, text) {
    var colorPicker = GetColorPicker(id);
    colorPicker.set_selectedColor(text);
}

function GetValue() {
    var colorPicker = GetColorPicker();
    return colorPicker.get_selectedColor();
}