function getDictionaryValue(array, key) {
    ///  
    /// Get the dictionary value from the array at the specified key  
    ///  
    var keyValue = key;
    var result;
    jQuery.each(array, function () {
        if (this.Key == keyValue) {
            result = this.Value;
            return false;
        }
    });
    return result;
}