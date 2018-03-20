/*
 UFrame 1.0.0
 
License:
	>Copyright (C) 2008 Omar AL Zabir - http://msmvps.com/blogs/omar
	>	
	>Permission is hereby granted, free of charge,
	>to any person obtaining a copy of this software and associated
	>documentation files (the "Software"),
	>to deal in the Software without restriction,
	>including without limitation the rights to use, copy, modify, merge,
	>publish, distribute, sublicense, and/or sell copies of the Software,
	>and to permit persons to whom the Software is furnished to do so,
	>subject to the following conditions:
	>
	>The above copyright notice and this permission notice shall be included
	>in all copies or substantial portions of the Software.
	>
	>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
	>INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	>FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	>IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
	>DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
	>ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
	>OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
 Usage:

 UFrame.init({
	    id: "iPanel1",  // id of the DIV
		
	    loadFrom: "somePage.aspx",  // Initial page to load from
	    initialLoad : "GET",    // Initial load mode
	    showProgress : true,    // Whether to show the progressTemplate
		
	    beforeLoad: function(url,data) { callback() },  // callback fired before content is loaded from loadFrom
	    afterLoad: function(data, response) { callback() }, // callback fired after response is loaded from loadFrom
	    beforePost: function(url,data) { callback() },  // callback fired before content is posted
	    afterPost: function(data, response) { callback() }, // callback fired after content is posted and response is available
		
	    params : { "Gaga" : "gugu" },   // parameters to post/get to 
		
	    progressTemplate : "<p>Loading...<p>",  // template shown when content is being loaded from or posted to
		
	    beforeBodyTemplate : "<p>This is rendered before the body</p>", // added before any response body
	    afterBodyTemplate : "<p>This is rendered after the body</p>",   // added after any response body
		
    });
*/

//NWC - Added 2010-12-16 to work around chrome issue.  Limits support to single uframe
var _frameContainer = '';
var _eventTarget;
var _eventArgument;
var _viewState;

(function(){

UFrame = function(config)
{
    this.config = config;
};

UFrame.prototype = {
    load : function()
    {
        var c = this.config;
        if( c.loadFrom )
        {
            var url = c.loadFrom;

            if(url.indexOf('?') > 0){
                url += '&uframe=1';
            }
            else{
                url += '?uframe=1';
            }
            UFrameManager.loadHtml(url, c.params, c);
        }
    },
    submit : function(form)
    {
        UFrameManager.submitForm(form, null);
    },
    navigate : function(href)
    {
        UFrameManager.loadHtml(href, null, this.config);
    }
}

UFrameManager =
{    
    _uFrames : {},
    
    empty : function() {},
    
    init : function(config) {
        _frameContainer = config.id;
        var o = new UFrame(config);
        UFrameManager._uFrames[config.id] = o;
        o.load();
    },
    getHtml : function(url, queryString, callback, method)
    {
        try 
        {                                           
            $.ajax({
                url:        url,
                type:       method || "GET",
                data:       queryString,
                dataType:   "html",
                success:    callback,
                error:      function(xml, status, e) 
                { 
                    //alert("Error occured while loading: " + url +'\n' + xml.status + ":" + xml.statusText + '\n' + xml.responseText); 
                    if( xml && xml.responseText )
                        callback(xml.responseText);
                    else
                        alert("Error occured while loading: " + url +'\n' + e.message);
                },
                cache:      true
            });
        } catch(e) { 
            alert(e);
        }
    },
    
    getUFrame : function(id)
    {
        return UFrameManager._uFrames[id];
    },

    //Prepare the outer form to be submitted.
    prepareOuterFormSubmit : function() 
    {
        //Remove all unnecessary asp.net hidden fields, which was added by the other pages.
        var first = $("#__VIEWSTATE");
        $("input[id='__VIEWSTATE']").each(function () {
            if ($(this).val() != first.val()) {
                $(this).remove();
            }
        });

        first = $("#__EVENTTARGET");
        $("input[id='__EVENTTARGET']").each(function () {
            if ($(this).val() != first.val()) {
                $(this).remove();
            }
        });

        first = $("#__EVENTARGUMENT");
        $("input[id='__EVENTARGUMENT']").each(function () {
            if ($(this).val() != first.val()) {
                $(this).remove();
            }
        });

        first = $("#__EVENTVALIDATION");
        $("input[id='__EVENTVALIDATION']").each(function () {
            if ($(this).val() != first.val()) {
                $(this).remove();
            }
        });     
    },
    
    isInit : function( ) {
        var uframe = UFrameManager.getUFrame(_frameContainer);
        return !(typeof(uframe) == 'undefined');
    },

    submitForm : function( form, submitData )
    {
        // Find all checked checkbox, radio button, text box, hidden fild, password box and submit button
        // collect all their names and values 
        var params = {};
	    $(form)
	        //.find("input[@checked], input[@type='text'], input[@type='hidden'], input[@type='password'], input[@type='submit'], option[@selected], textarea")
	        .find("input:checked, input[type='text'], input[type='hidden'], input[type='password'], option:selected, textarea")
            .filter(":not(:disabled)")  // :enabled cannot be used because it excepts hidden fields
	        .each(function() {
//                Commented 2010-12-30 - textarea isn't handled properly. html() returns the old value.
//                if(this.nodeName.toLowerCase() == 'textarea'){
//                    params[ this.name || this.id || this.parentNode.name || this.parentNode.id ] = $(this).html();
//                }
//                else{
		            params[ this.name || this.id || this.parentNode.name || this.parentNode.id || this.parentNode.parentNode.name || this.parentNode.parentNode.id ] = this.value;
//                }
	        });
        
	    if( submitData )
	        params[ submitData.name ] = submitData.value;

        //NWC 2010-12-16
        // Add ASP.NET fields
        if(_eventTarget){
            params['__EVENTTARGET'] = _eventTarget;
        }

        if(_eventArgument){
            params['__EVENTARGUMENT'] = _eventArgument;
        }

        if(_viewState){
            params['__VIEWSTATE'] = _viewState;
        }
	    
        //NWC 2010-12-16 Chrome Change
	    //var uframeId = $(form).attr("UFrameID");
        var uframeId = _frameContainer;
        //

	    var uframe = UFrameManager.getUFrame(uframeId);
	    
        if (typeof(uframe) == 'undefined')
            return;

	    var config = uframe.config;
	    var container = $('#' + config.id);
        
        //NWC 2010-12-16 Chrome Change
        //        var url = UFrameManager.resolveUrl( config.loadFrom, form.action );
        var url = config.loadFrom;
        //

        if((config.beforeLoad || UFrameManager.empty)(url, params) !== false)
        {
            if(config.progressTemplate) container.html(config.progressTemplate);
                
	        UFrameManager.getHtml(url, params, function(data)
	        {
	            config.loadFrom = url;
	            (config.afterLoad || UFrameManager.empty)(url, data);
		        UFrameManager.processHtml(data, container, config);
	        }, "POST");
	    }
    },
    
    loadHtml : function(url, params, config)
    {
        var container = $('#' + config.id);
        var queryString = $.param(params || {});

        if((config.beforeLoad || UFrameManager.empty)(url, params) !== false)
        {
            if(config.progressTemplate) container.html(config.progressTemplate);
            config.loadFrom = url;
            UFrameManager.getHtml(url, queryString, function(content)
            {
                (config.afterLoad || UFrameManager.empty)(url, content);
                UFrameManager.processHtml(content, container, config);
            });
        }
    },

    processHtml : function(content, container, config)
    {
        var result = UFrameManager.parseHtml(content, config);
            
        var head = document.getElementsByTagName('head')[0];
        
        $(result.styles).each(function(index,text)
        {                
            var styleNode = document.createElement("style");
	        styleNode.setAttribute("type", "text/css");
	        if(styleNode.styleSheet) // IE
	        {
		        styleNode.styleSheet.cssText = text;
	        } 
	        else // w3c
	        {
		        var cssText = document.createTextNode(text);
		        styleNode.appendChild(cssText);
	        }
			
	        head.appendChild(styleNode);
        });
        
        $(result.links).each(function(index,attrs)
        {
            window.setTimeout(function()
            {                
                var link = document.createElement('link');
                var href = "";
                for( var i = 0; i < attrs.length; i ++ )
                {
                    var attr = attrs[i];
                    if( attr.name == "href" ) href = attr.value;
                    link.setAttribute("" + attr.name, "" + attr.value);
                }
                
                if( href.length > 0 )
                { 
                    var newUrl = href.split('?')[0];
                    link.href = UFrameManager.resolveUrl( config.loadFrom, href );
                    if( !UFrameManager.isTagLoaded('link', 'href', link.href) )
                    {
                        if (newUrl != null && newUrl.length > 0)
                        {
                            $(head).children().each(function(idx, obj)
                            {
                                var tmpHref = $(obj).attr("href");
                                if (tmpHref != null && tmpHref.length > 0)
                                { 
                                    if (tmpHref.indexOf(newUrl) > -1)
                                    {
                                        head.removeChild(obj);
                                    }
                                }
                            });
                        }
                        head.appendChild(link);
                    }
                }
            }, 0);
        });

        var scriptsToLoad = result.externalScripts.length;
        
        $(result.externalScripts).each(function(index, scriptSrc)
        {
            scriptSrc = UFrameManager.resolveUrl( config.loadFrom, scriptSrc );
            
            if(UFrameManager.isTagLoaded('script', 'src', scriptSrc))
            {
                scriptsToLoad --;                
            }
            else
            {
                $.ajax({ 
                    url:        scriptSrc,
                    type:       "GET",
                    data:       null,
                    dataType:   "script",
                    success:    function(){ scriptsToLoad--; },
                    error:      function(){ scriptsToLoad--; },
                    cache:      false
                });
            }
        });        
        
        // wait until all the external scripts are downloaded
        
        UFrameManager.until({ 
            test:       function() { return scriptsToLoad === 0; }, 
            delay:      100,
            callback:   function()
                        {
                            // render the body
                            var html = (config.beforeBodyTemplate||"") + result.body + (config.afterBodyTemplate||"");

                            //Replace method 'html' with 'append', because the first one excluded the tag 'form'
                            container.empty();//.html('');
                            container.append(html);
           
                            window.setTimeout( function()
                            {
                                // execute all inline scripts 
                                $(result.inlineScripts).each(function(index, script)
                                { 
                                   try {
                                       $.globalEval(script);
                                   }
                                    catch(err) {
                                       //alert('UFrame Eval Errror: ' + err.description);
                                       //alert(script);
                                   }
                                });                            
                                
                                UFrameManager.hook(container, config);
                                
                                if( typeof callback == "function" ) callback();
                            }, 0 );
                        }
        });
        
        
    },
    
    isTagLoaded : function(tagName, attName, value)
    {
        // Create a temporary tag to see what value browser eventually 
        // gives to the attribute after doing necessary encoding
        var tag = document.createElement(tagName);
        tag[attName] = value;
        var tagFound = false;
        $(tagName,document).each(function(index,t) 
        { 
            if( tag[attName] === t[attName] ) { tagFound = true; return false } 
        });
        return tagFound;
    },
    
    hook : function(container, config)
    {
        // Add an onclick event on all <a> 
        $("a:not([uframeignore], [returnfile])", container)
        .unbind("click")
        .click(function() 
        { 
            //If href is disabled, leave it
            if ($(this).attr('disabled'))
                return true;

            var href = $(this).attr("href");
            if( href )
            {
                // if href is an absolute URL to outside domain, leave it
                if( href.indexOf("http://") >= 0 && href.indexOf(document.location.host) < 0 )
                {
                    return true;
                }
                else
                {
                    if (href.indexOf('javascript:') !== 0) 
                    {
                        UFrameManager.loadHtml(href, null, config);
                        return false;
                    }
                    else if(UFrameManager.executeASPNETPostback(this, href))
                    {
                        return false;
                    }
                    else
                        return true;
                }
            }
            else
            {
                return true;
            }
        });
        
        // Hook all button type things that can post the form
        $(":image,:submit,:button", container)
            .not('[uframeignore], [returnfile]')
            .unbind("click")
            .click(function() 
            { 
                return UFrameManager.submitInput(this); 
            });
        
        // Hook select dropdown with onselectchange postback        
        var dropDownItems = $("select:not([uframeignore], [returnfile], [uniformIgnore])", container);        
        dropDownItems
            .unbind("change")            
            .change(function() 
            {
                //Only submit form if autopostback is true
                if($(this).attr('onChange') != null && $(this).attr('onChange') != '')
                    return UFrameManager.submitInput(this);

                //var c1 = $(this).attr('onChange');// c1 = 'undefined'
                //var c2 = $(this).attr('onchange');// c2 = null

                // IE hack
                if(navigator.appName == "Microsoft Internet Explorer" )
                    return UFrameManager.submitInput(this);
            })
            .each(function()
            {
                this.onchange = null;
            });
        
        //The previos operation unbinds 'change' event on select elements. It causes errors in jQuery uniform plugin,
        //because the last one binds 'change' event on these items. So the uniform plugin functionality should be restored.
        if ($.uniform && $.uniform.restore){
            $.uniform.restore(dropDownItems);
            dropDownItems.uniform();
        }
        
        // Only for IE6 : enter key invokes submit event
        $("form", container)
            .attr("UFrameID", config.id)
            .unbind("submit")
            .submit(function() {
                var firstInput = $(":image,:submit,:button", container).get(0);
			    return UFrameManager.submitInput(firstInput);
            } );
            

        //Add 'click' handler to all elements with the attribute 'onclick'
        $("input[onclick]", container)
        .not('[uframeignore],[returnfile],:image,:submit,:button')
        .unbind('click')        
        .click(function()
        {           
            var href = $(this).data('oldclick').toString();

            //Remove unnecessary part of href.
            var startInd = href.indexOf("__doPostBack");
            if (startInd < 0)
                startInd = href.indexOf("WebForm_DoPostBackWithOptions");
            href = href.substr(startInd);

            //Remove all \ characters
            href = href.replace(/\\/g,'');
            
            //Make AspNet Postback via AJAX
            UFrameManager.executeASPNETPostback(this, href)
            return false;
        })
        .each(function()
        { 
            $(this).data('oldclick', $(this).attr('onclick'));
            this.onclick = null;
        });     

        //If the response is file, make POST request without AJAX.
        $("[returnfile]", container)
        .not('[uframeignore]')
        .unbind('click')
        .click(function()
        {
            var href = $(this).attr('href');

            if(href == null || href == ''){
                return;
            }

            //Replace " with '
            href = href.replace(/"/g,'\'');

            if ($(this).attr('returnfile')=="true")
            {
                //Make Post request without AJAX
                var parts = href.split("'");
                var eventTarget = parts[1];
                var eventArgument = parts[3];

                var form = $(this).parents("form").get(0);

                if (!form.onsubmit || (form.onsubmit() != false)) {
                    form.__EVENTTARGET.value = eventTarget;
                    form.__EVENTARGUMENT.value = eventArgument;

                    form.submit();
                }
            } 
            else
            {
                //Make AspNet Postback via AJAX
                UFrameManager.executeASPNETPostback(this, href)
            }
        });
    
    },

    saveTinyMCE : function()
    {
        //tinymce.triggerSave();
        if (typeof(tinymce) != 'undefined')
        {
		    $(tinymce.editors).each(function(index, inst) {                
                try
                {
                    if (!($('#' + inst.id.replace(new RegExp('_questionText$'), '_currentEditMode')).val().toLowerCase() == 'textarea'))
                    {
                        inst.save();
                    }
                }
                catch (ee) //suppress strange exception from jquery
                {
                }
		    });
        }
    },
    
    executeASPNETPostback : function(input, href)
    {
        UFrameManager.saveTinyMCE();

        if((href.indexOf("__doPostBack") >= 0) || (href.indexOf("WebForm_DoPostBackWithOptions") >= 0))
        {
            //Replace " with '
            href = href.replace(/"/g,'\'');

            // ASP.NET Postback. Collect the values being posted and submit them manually
            var parts = href.split("'");
            var eventTarget = parts[1];
            var eventArgument = parts[3];
            
            var form = $(input).parents("form").get(0);

            //NWC 2010-12-16 - More Hackery for Chrome
            //form.__EVENTARGUMENT.value = unescape(eventArgument);
            //form.__EVENTTARGET.value = unescape(eventTarget);
            _eventArgument = unescape(eventArgument);
            _eventTarget = unescape(eventTarget);
            
            if(form.__VIEWSTATE){
                if(form.__VIEWSTATE.length == null){
                    _viewState = form.__VIEWSTATE.value;
                }
                else{
                   _viewState = form.__VIEWSTATE[1].value;
                }
            }

            //end hackery

            UFrameManager.submitForm( form, null );
            return true;
        }
        else
        {
            return false;
        }
    },
    
    submitInput : function(input)
    {
        var form = $(input).parents("form").get(0);
        if (typeof form == "undefined" || (form.onsubmit && form.onsubmit() == false) ) 
        {
            return false;
        }
        
        input = $(input);        
        
        _eventTarget = input.attr("name");
        _viewState = form.__VIEWSTATE.value;

        UFrameManager.submitForm( form, { name: input.attr("name"), value: $(input).val() } );
        
        return false;
    },
    
    until : function(o /* o = { test: function(){...}, delay:100, callback: function(){...} } */)
    {
        if( o.test() === true ) o.callback();
        else window.setTimeout( function() { UFrameManager.until(o); }, o.delay || 100);
    },
    
    delay : function( func, delay )
    {
        window.setTimeout( func, delay || 100 );
    },
    
    resolveUrl : function( baseUrl, relativeUrl )
    {
        // Hack for firefox, as Firefox make any relative URL absolute by just adding the current document's absolute path
        // to the URL
        var currentPageUrl = document.location.protocol + "//" + document.location.host + document.location.pathname;
            
        if( relativeUrl.indexOf(currentPageUrl) == 0) relativeUrl = relativeUrl.substring(currentPageUrl.length);
        
        // if it's an absolute URL, return as it is
        if( relativeUrl.indexOf("http://") >= 0 ) return relativeUrl;
        
        // If URL starts with root, then return it as it is
        if( relativeUrl.indexOf("/") == 0 ) return relativeUrl;
        
        var lastSeparator = baseUrl.lastIndexOf("/");
        if( lastSeparator < 0 ) return relativeUrl;
        else return baseUrl.substring(0, lastSeparator) + "/" + relativeUrl;
    },
    
    parseHtml : function(content)
    {
        var result = { body : "", externalScripts : [], inlineScripts : [], links : [], styles : [] };
        
        var bodyContent = [];
        var bodyStarted = false;
        
        var inlineScriptStarted = false;            
        var inlineScriptContent = [];
        
        var inlineStyleStarted = false;            
        var inlineStyleContent = [];

        var inlineTemplateStarted = false;
        
        HTMLParser(content, {
          start: function( tag, attrs, unary ) 
          {
                if( tag == "body" )
                {
                    bodyStarted = true;
                }
                else if(tag == "script")
                {//text/x-jquery-tmpl
                    var isTmpl = false;

                    $(attrs).each(function(index, attr) {
                        if (attr.name == 'type' && attr.value == 'text/x-jquery-tmpl') {
                            isTmpl = true;
                        }
                    });
                    
                    var srcFound = false;
                    $(attrs).each(function(index,attr)
                    {                        
                        if(attr.name == "src")
                        {
                            result.externalScripts.push(attr.value);
                            srcFound = true;
                        }
                    });
                    
                    if(isTmpl)
                    {
                        var attributes = [];
                        for ( var i = 0; i < attrs.length; i++ ) 
                            attributes.push(attrs[i].name + '="' + attrs[i].value + '"');
                            
                        bodyContent.push("<" + tag + " " + attributes.join(" ") + (unary ? "/" : "") + ">");
                        inlineTemplateStarted = true;
                    }
                    else if( !srcFound)
                    {
                        // inline script
                        inlineScriptStarted = true;
                        inlineScriptContent = [];
                    }
                }
                else if(tag == "link")
                {
                    result.links.push(attrs);
                }
                else if(tag == "style")
                {
                    // inline style node
                    inlineStyleStarted = true;
                    inlineStyleContent = [];
                }
                else
                {
                    if( bodyStarted )
                    {
                        var attributes = [];
                        for ( var i = 0; i < attrs.length; i++ ) 
                            attributes.push(attrs[i].name + '="' + attrs[i].value + '"');
                            
                        bodyContent.push("<" + tag + " " + attributes.join(" ") + (unary ? "/" : "") + ">");
                    }
                }
          },
          end: function( tag ) 
          {
            if( tag == "script" )
            { 
                if(inlineTemplateStarted) {
                    inlineTemplateStarted = false;
                    bodyContent.push("</script>");
                }
                else if( inlineScriptStarted )
                {
                    inlineScriptStarted = false;
                    result.inlineScripts.push( inlineScriptContent.join("\n") );
                }
            }
            else if( tag == "style" )
            {
                inlineStyleStarted = false;
                result.styles.push( inlineStyleContent.join("\n") );
            }
            else
            {
                if( bodyStarted )
                    bodyContent.push("</" + tag + ">");
            }
          },
          chars: function( text ) 
          {
            if( inlineScriptStarted ) 
                inlineScriptContent.push(text);
            else if( inlineStyleStarted ) 
                inlineStyleContent.push(text);
            else if( bodyStarted ) 
                bodyContent.push(text);
          },
          comment: function( text ) 
          {
          }
        });
        
        result.body = bodyContent.join("\n");
        return result;
    },
    initContainers : function()
    {
        $('div[src]',document).each(function()
        {
            var container = $(this);
            var id = container.attr("id");
            if( null == UFrameManager._uFrames[id] )
            {
                UFrameManager.init({
                    id: id,
    		
	                loadFrom: container.attr("src"),
	                initialLoad : "GET",
    	            
	                progressTemplate : container.attr("progressTemplate") || null,
    	            
                    showProgress : container.attr("showProgress") || false,
    		
	                beforeLoad: function(url,data) { return eval(container.attr("beforeLoad") || "true") },
	                afterLoad: function(data, response) { return eval(container.attr("afterLoad") || "true") },
	                beforePost: function(url,data) { return eval(container.attr("beforePost") || "true") },
	                afterPost: function(data, response) { return eval(container.attr("afterPost") || "true") },
            		
	                params : null,
            		
	                beforeBodyTemplate : container.attr("beforeBodyTemplate") || null,		
	                afterBodyTemplate : container.attr("afterBodyTemplate") || null
                });
            }
        });
    }
}

$(function()
{
    UFrameManager.initContainers();
});

})();
