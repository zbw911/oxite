﻿window._emailRegex = /^[a-z0-9]+([-+\.]*[a-z0-9]+)*@[a-z0-9]+([-\.][a-z0-9]+)*$/i;

/** field hinting **/
$(document).ready(function() {
    $("form").each(function() { $(this).hintify() });
});
$.extend(jQuery.expr[":"], {
    textarea: function(a) { return $.nodeName(a, 'textarea'); }
});
$.fn.extend({
    hintify: function() {
        this.submit(function() {
            $("[_hint]", this).each(function() { $(this).removeHint() });
        });

        $(window).unload(function() {
            $("form [_hint]").each(function() { $(this).removeHint() });
        });

        $(":text[title],:textarea[title]", this).filter(":enabled").each(function() { $(this).hint() });

        return this;
    },
    hint: function() {
        var hintText = this.attr("title");
        if (!!hintText && (this.is(":text") || this.is(":textarea"))) {
            this.attr("_hint", hintText);
            this.addHint()
            this.focus(function() { $(this).removeHint(); });
            this.blur(function() { $(this).addHint(); });
        }
        return this;
    },
    addHint: function() {
        if ($.trim(this.val()) === "") {
            this.addClass("hinted");
            this.removeClass("active");
            this.val(this.attr("_hint"));
        } else {
            this.addClass("active");
        }
    },
    removeHint: function() {
        if ($.trim(this.val()) === this.attr("_hint")) {
            this.val("");
            this.removeClass("hinted");
        }
        this.addClass("active");
    }
});

/*** gravatar fetch and alt change ***/
$(document).ready(function() {
    $('#comment_email').blur(function() {
        var email = $(this).val();
        if (email.indexOf("@") > 0 && window._emailRegex.test(email)) {
            $.post(window.computeHashPath, { value: email }, function(emailHash) { if (emailHash && emailHash.length < 50) { $('#comment_grav').changeGravatarSrcTo(emailHash); } });
        } else {
            var emailHash = $('#comment_hashedEmail') ? $('#comment_hashedEmail').val() : "";
            $('#comment_grav').changeGravatarSrcTo(emailHash);
        }
    });
    $('#comment_name').blur(function() {
        $('#comment_grav').changeGravatarAltTo($(this).val());
    });
});
$.fn.extend({
    changeGravatarSrcTo: function(emailHash) {
        var gravatar = $(this).find("img.gravatar");

        var gravatarUrlParts = gravatar.attr("src").split("?");
        var gravatarPathParts = gravatarUrlParts[0].split("/");

        gravatarPathParts[gravatarPathParts.length - 1] = emailHash;

        gravatar.attr("src", gravatarPathParts.join("/") + "?" + gravatarUrlParts[1]);
    },
    changeGravatarAltTo: function(name) {
        var gravatar = $(this).find("img.gravatar");
        if ($.trim(name) !== "") {
            gravatar.attr("title", name + " (gravatar)");
        } else {
            gravatar.attr("title", "(gravatar)");
        }
    }
});

/** username in the login form gets focus on load **/
$(document).ready(function() {
    $("#login_username").focus();
});

/** archives **/
$(document).ready(function() {
    $('.archives ul.yearList li.previous').each(function() {
        $(this).click(function(ev) {
            if (!ev || $(ev.target).not("a").size()) {
                $(this).toggleClass("open");
                $(this).find("h4>span").toggle();
                $(this).children("ul").toggle();
            }
        });

        $(this).hoverClassIfy();
    });
});

/** list item highlighting - just comma seperate additional selectors for now because we like to try to make the browser work **/
$(document).ready(function() {
    $("ul.small li.comment,ul.small li.post,ul.medium li.comment.pendingapproval,ul.medium li.comment.normal").each(function() {
        $(this).hoverClassIfy();
        $(this).clickClassIfy();
    });
});
$.fn.extend({
    hoverClassIfy: function() {
        $(this).mouseover(function() {
            $(this).addClass("hover");
        });

        $(this).mouseout(function() {
            $(this).removeClass("hover");
        });

        return this;
    },
    clickClassIfy: function() {
        $(this).click(function(ev) {
            if (!($(ev.target).is("a"))) {
                $(this).toggleClass("active");
            }
        });
    }
});

/** flags **/
$(document).ready(function() {
    /* removal */
    $("form.remove.post").submit(function() {
        return window.confirm('really?');
    });
    $("form.flag.remove").submit(function() {
        var form = $(this);
        var comment = $(this).offsetParent("li.comment");
        comment.fadeTo(350, .4);
        $.ajax({
            type: "POST",
            url: this.action,
            data: { id: this.id.value, __RequestVerificationToken: this.__RequestVerificationToken.value },
            success: function(response) {
                if (response === "true") {
                    comment.animate({ height: 0, opacity: 0, marginTop: 0, marginBottom: 0, paddingTop: 0, paddingBottom: 0 }, 200); form = comment = 0;
                } else {
                    this.error();
                }
            },
            error: function() { comment.fadeTo(350, 1); form = comment = 0; }
        });
        return false;
    });
    /* approval */
    $("form.flag.approve").submit(function() {
        var form = $(this);
        var markers = $(".approve,.state", $(this).offsetParent("li.comment"));
        markers.fadeTo(350, .4);
        $.ajax({
            type: "POST",
            url: this.action,
            data: { id: this.id.value, __RequestVerificationToken: this.__RequestVerificationToken.value },
            success: function(response) {
                if (response === "true") {
                    markers.hide(200); form = markers = 0;
                } else {
                    this.error();
                }
            },
            error: function() { markers.fadeTo(350, 1); form = markers = 0; }
        });
        return false;
    });
    /* removeFile */
    $("form.flag.removeFile").submit(function() {
        var form = $(this);
        var file = $(this).parent("div.manageFile");
        file.fadeTo(350, .4);
        $.ajax({
            type: "POST",
            url: this.action,
            data: { fileUrl: this.fileUrl.value, __RequestVerificationToken: this.__RequestVerificationToken.value },
            success: function(response) {
                if (response === "true") {
                    file.hide(200);
                } else {
                    this.error();
                }
            },
            error: function() { file.fadeTo(350, 1); form = file = 0; }
        });
        return false;
    });
});

/** highlight anchored element **/
$(document).ready(function() {
    var hash = window.location.hash;
    if (hash) {
        $(hash).each(function() { $(this).highlight() });
    }
});
/* really, really simple implementation. some todos:
    - listen to all hashed hrefs on the page so the highlight can change
    - make some time to think of other todos :P */
$.fn.extend({ 
    highlight: function(highlightColor) {
        this.addClass("highlight");
    }
});

/* comment on a comment */
$(document).ready(function() {
    var commentForm = $("form#comment");
//    $("li.comment").each(function() { $(this).enableReply(commentForm) });
});

$.fn.extend({
    enableReply: function(commentForm) {
        //todo: (nheskew) hook it up!
        $(this).html($(this).html() + " <a href=\"#\">reply</a>");
    }
});


// some duplicate stuff from the admin. undo later :|

/** async content loading and handling redirects **/
jQuery.extend({
    _ajax: jQuery.ajax,
    ajax: function(options) {
        var complete = options.complete;

        options.complete = function(response, status) {
            var responseText = response && response.responseText;

            if (responseText && response.getResponseHeader("Content-Type") && "application/json" == response.getResponseHeader("Content-Type").split(";")[0].toLowerCase()) {
                var data = $.httpData(response, "json", options);

                if (data) {
                    if (data.cancel) {
                        status = "cancelled";
                    }

                    if (typeof data.url === "string") {
                        document.location = data.url;
                        return false;
                    }
                }
            } else if (responseText && response.getResponseHeader("X-Oxite-Dialog")) {
                return $(responseText).lightbox(function(response, status) {
                    if (typeof complete === "function") {
                        complete(response, status);
                    }
                    complete = 0;
                });
            }

            if (typeof complete === "function") {
                complete(response, status);
                complete = 0;
            }
        }

        return jQuery._ajax(options);
    }
});
$.fn.extend({
    /* quick and brutal :| */
    getDataArray: function(existing) {
        var data = [];
        var elements = $("input[name]", this);

        elements.each(function() {
            var element = $(this);
            if (element.attr("name") !== "returnUri" && (!!(element.val()) || element.val() === "" || element.val() === "0")) {
                data.push({ name: element.attr("name"), value: element.val() });
            }
        });

        return existing instanceof Array ? $.merge(existing, data) : data;
    }
});
