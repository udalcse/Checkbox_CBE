
// jQuery contains case-insensitive extension
$.extend($.expr[":"], {
    "containsIgnoreCase": function (elem, i, match, array) {
        return (elem.textContent || elem.innerText || "").toLowerCase().indexOf((match[3] || "").toLowerCase()) >= 0;
    }
});

// User Store Selection
var userStoreSelectSel = '.userStoreSelect';
var customSelectValueSel = '.custom-select-value';
$(document).on('click', userStoreSelectSel + ' ' + customSelectValueSel, function (e) {
    var $menu = $(this).closest(userStoreSelectSel).find('.custom-select');
    $menu.show();
});
$(document).on('click', userStoreSelectSel + ' .custom-select li a', function (e) {
    var $menu = $(this).closest('.custom-select');
    var $container = $menu.closest(userStoreSelectSel);
    var selValueText = $(this).text();
    var selValue = $(this).attr('data-select-value');
    var selName = $menu.attr('data-select-name');
    $container.find('select[name="' + selName + '"]').val(selValue);
    $menu.closest(userStoreSelectSel).find(customSelectValueSel).text(selValueText);
    $menu.hide();
});
//close store selection dropdown
$("body").click(function (e) {
    $(".custom-select-container .custom-select").hide();
});


// Clickable select list transfer UI
var SelectListTransfer = function () {
    var self = this;

    var $availableFilter;
    var $availableSelect;
    var $availableList;
    var $selectedFilter;
    var $selectedSelect;
    var $selectedList;

    function rezebraLists() {
        var srcCount = 0;
        $availableList.find('li').each(function () {
            var zClass = srcCount % 2 == 0 ? 'zebra' : 'detailZebra';
            $(this).attr('class', 'gridContent ' + zClass);
            srcCount++;
        });

        var destCount = 0;
        $selectedList.find('li').each(function () {
            var zClass = destCount % 2 == 0 ? 'zebra' : 'detailZebra';
            $(this).attr('class', 'gridContent ' + zClass);
            destCount++;
        });
    }

    function renderClickableListItem(type, $selItem) {
        var $srcList = type == 'available' ? $availableList : $selectedList;
        var destType = type == 'available' ? 'selected' : 'available';
        var $listItem = $('<li class="gridContent" data-type="' + type + '" data-value="' + $selItem.val() + '"><a href="#">' + $selItem.html() + '</a></li>');

        $srcList.append($listItem);
        rezebraLists();

        $listItem.click(function (e) {
            var val = $(this).closest('li').attr('data-value');
            var type = $(this).closest('li').attr('data-type');
            var $srcSelect = type === 'available' ? $availableSelect : $selectedSelect;
            var option = $srcSelect.find('option[value="' + val + '"]').remove();
            $srcSelect = type !== 'available' ? $availableSelect : $selectedSelect;
            $srcSelect.append(option);

            renderClickableListItem(destType, $selItem);
            $(this).closest('li').remove();
            rezebraLists();
        });
    }

    function initClickableList(type) {
        var $select = type == 'available' ? $availableSelect : $selectedSelect;
        $select.find('option').each(function () {
            renderClickableListItem(type, $(this));
        });

        var $list = type == 'available' ? $availableList : $selectedList;
        var $filter = type == 'available' ? $availableFilter : $selectedFilter;
        $filter.keyup(function (e) {
            $list.find('li').hide();
            $list.find('li:containsIgnoreCase("' + $(this).val() + '")').show();
        });
        var $filterClear = $filter.next('input[type="image"]');
        if ($filterClear.length > 0) {
            $filterClear.click(function (e) {
                $list.find('li').show();
            });
        }
    }

    self.init = function (srcSelectId, srcListId, srcFilterId, destSelectId, destListId, destFilterId) {
        $availableSelect = $('#' + srcSelectId);
        $availableList = $('#' + srcListId);
        $availableFilter = $('#' + srcFilterId);
        $selectedSelect = $('#' + destSelectId);
        $selectedList = $('#' + destListId);
        $selectedFilter = $('#' + destFilterId);

        initClickableList('available');
        initClickableList('selected');
    };
}