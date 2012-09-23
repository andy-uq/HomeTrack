jQuery.ajaxSettings.traditional = true;

ko.bindingHandlers['class'] = {
	'update': function (element, valueAccessor) {
		if (element['__ko__previousClassValue__']) {
			$(element).removeClass(element['__ko__previousClassValue__']);
		}
		var value = ko.utils.unwrapObservable(valueAccessor());
		$(element).addClass(value);
		element['__ko__previousClassValue__'] = value;
	}
};