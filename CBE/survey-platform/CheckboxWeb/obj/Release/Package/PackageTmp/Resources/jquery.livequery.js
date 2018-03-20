/*
Copyright
(c) 2010, Brandon Aaron (http://brandonaaron.net/)
(c) 2012 - 2013, Alexander Zaytsev (http://hazzik.ru/en)
Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:
The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
!function(e){"function"==typeof define&&define.amd?define(["jquery"],e):e("object"==typeof exports?require("jquery"):jQuery)}(function(e,n){function t(e,n,t,u){return!(e.selector!=n.selector||e.context!=n.context||t&&t.$lqguid!=n.fn.$lqguid||u&&u.$lqguid!=n.fn2.$lqguid)}e.extend(e.fn,{livequery:function(n,r){var i,o=this;return e.each(u.queries,function(e,u){return t(o,u,n,r)?(i=u)&&!1:void 0}),i=i||new u(o.selector,o.context,n,r),i.stopped=!1,i.run(),o},expire:function(n,r){var i=this;return e.each(u.queries,function(e,o){t(i,o,n,r)&&!i.stopped&&u.stop(o.id)}),i}});var u=e.livequery=function(n,t,r,i){var o=this;return o.selector=n,o.context=t,o.fn=r,o.fn2=i,o.elements=e([]),o.stopped=!1,o.id=u.queries.push(o)-1,r.$lqguid=r.$lqguid||u.guid++,i&&(i.$lqguid=i.$lqguid||u.guid++),o};u.prototype={stop:function(){var n=this;n.stopped||(n.fn2&&n.elements.each(n.fn2),n.elements=e([]),n.stopped=!0)},run:function(){var n=this;if(!n.stopped){var t=n.elements,u=e(n.selector,n.context),r=u.not(t),i=t.not(u);n.elements=u,r.each(n.fn),n.fn2&&i.each(n.fn2)}}},e.extend(u,{guid:0,queries:[],queue:[],running:!1,timeout:null,registered:[],checkQueue:function(){if(u.running&&u.queue.length)for(var e=u.queue.length;e--;)u.queries[u.queue.shift()].run()},pause:function(){u.running=!1},play:function(){u.running=!0,u.run()},registerPlugin:function(){e.each(arguments,function(n,t){if(e.fn[t]&&!(e.inArray(t,u.registered)>0)){var r=e.fn[t];e.fn[t]=function(){var e=r.apply(this,arguments);return u.run(),e},u.registered.push(t)}})},run:function(t){t!==n?e.inArray(t,u.queue)<0&&u.queue.push(t):e.each(u.queries,function(n){e.inArray(n,u.queue)<0&&u.queue.push(n)}),u.timeout&&clearTimeout(u.timeout),u.timeout=setTimeout(u.checkQueue,20)},stop:function(t){t!==n?u.queries[t].stop():e.each(u.queries,u.prototype.stop)}}),u.registerPlugin("append","prepend","after","before","wrap","attr","removeAttr","addClass","removeClass","toggleClass","empty","remove","html","prop","removeProp"),e(function(){u.play()})});