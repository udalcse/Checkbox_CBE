This folder contains the langauge resources for the XHEO|Licensing runtime. In order to use the language in your application you must build the Xheo.Licensing.[language].resx file of the matching language to a subfolder of your applications executable folder. This subfolder must match the language it represents.

For example, to use the Italian translation in your code you would add the Xheo.Licensing.it.resx file from the \it folder to your project. 

\Application Root
	\langauge
		Xheo.Licensing.resources.dll
	\it
		Xheo.Licensing.resources.dll
	\es
		Xheo.Licensing.resources.dll
		
		
You can use the enclosed Xheo.Licensing.resx file to create your own translation. When using your own translation you must add the translated RESX file to your assembly in a Resources folder off the main project directory. The resulting manifest name for the resources must be [AssemblyName].Resources.Xheo.Licensing.resources.

See the online help for more details:

http://www.xheo.com/xdn/documentation/Default.aspx?topic=xheo.licensing/html/backgrounders/multilanguage.html