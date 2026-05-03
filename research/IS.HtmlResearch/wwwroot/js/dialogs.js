class TextSettings {
    static TypeDelay = 5;
}

class CssHelper {
    static AddOutClass(element) {
        var css = element.attr("class");
        element.attr("class", "");
        element.get(0).offsetWidth;
        element.attr("class", css + " out");
    }
}

class Baloon {
    constructor() {
        this.scene = void 0;
        this.balloon = void 0;
        this.textContainer = void 0;
        this.titleContainer = void 0;
    }

    ChangeTitle(title) {
        if (this.titleContainer !== void 0) {
            this.titleContainer.html(title);
        }
    }

    ShowText(text) {
        if (this.balloon === void 0 || this.textContainer === void 0) {
            return;
        }

        this.balloon.show();
        TypeWrite(this.textContainer, text);

        function TypeWrite(container, text) {
            container.empty();

            var temp = $("<span>" + text + "</span>");

            var timeouts = [];
            var currentElement = $("<span />").appendTo(container);

            temp.each(function () {
                ProcessChild(currentElement, $(this), timeouts);
            });

            function ProcessChild(currentElement, element, timeouts) {
                element.contents().each(function () {
                    if (this.nodeType == 3) {
                        var contents = $(this).text();

                        var newSpans = $('<span>' + contents.split('').join('</span><span>') + '</span>');
                        newSpans.hide().appendTo(currentElement).each(function (i) {
                            var span = $(this);
                            span.css({ display: 'inline', opacity: 0 });
                            timeouts.push(setTimeout(function () {
                                span.animate({ opacity: 1 }, TextSettings.TypeDelay);
                            }, TextSettings.TypeDelay * timeouts.length));
                        });
                    } else {
                        var tagName = $(this).prop("tagName").toLowerCase();
                        var child = $("<" + tagName + " />");
                        child.appendTo(currentElement);
                        ProcessChild(child, $(this), timeouts);
                    }
                });
            }
        }
    }

    Hide() {
        var me = this;
        if (me.scene === void 0) {
            return;
        }
        me.balloon.hide();
        CssHelper.AddOutClass(me.scene);
    };

    Remove() {
        if (this.scene !== void 0) {
            this.scene.remove();
            this.scene = void 0;
        }
    }
}

const Emotion = {
    NORMAL: "normal",
    HAPPY: "happy",
    SAD: "sad",
    SURPRISED: "surprised",
    ANGRY: "angry"
};

class PersonBaloon extends Baloon {
    constructor(sceneClass, directionClass, imageClass) {
        super();
        this.balloon = void 0;
        this.emotion = Emotion.NORMAL;
        this.sceneClass = sceneClass;
        this.directionClass = directionClass;
        this.imageClass = imageClass;
        this.voiceArrowHtml = void 0;
        this.thoughtArrowHtml = void 0;
        this.arrowContainer = void 0;
    }

    Show(container, emotion, person, title) {
        this.person = person;

        this.scene = $(
            '<div class="scene ' + this.sceneClass + '">' +
            '<div class="personContainer">' +
            '<div class="personSizer ' + this.directionClass + '">' +
            '</div>' +
            '</div>' +
            '<div class="dialogContainer">' +
            '<div class="baloon ' + this.directionClass + '">' +
            '<div class="baloonTitle">' +
            '<span>' + title + '</span>' +
            '</div>' +
            '<svg class="baloonArrow ' + this.directionClass + '"></svg>' +
            '<div class="baloonText withTitle"></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

        this.titleContainer = this.scene.find(".baloonTitle span");
        this.personContainer = this.scene.find(".personSizer");
        this.ChangeEmotion(emotion);
        this.balloon = this.scene.find(".dialogContainer").hide();
        this.arrowContainer = this.scene.find(".baloonArrow");
        this.textContainer = this.scene.find(".baloonText");
        this.scene.appendTo(container);
    };

    ShowVoice() {
        this.textContainer.empty();
        this.arrowContainer.html(this.voiceArrowHtml);
    }

    ShowThought() {
        this.textContainer.empty();
        this.arrowContainer.html(this.thoughtArrowHtml);
    }

    ChangeEmotion(emotion) {
        var me = this;

        if (me.oldEmotion !== void 0) {
            ClearField("oldEmotion");
            ClearField("oldPerson");
        }

        if (me.currentEmotion !== void 0) {
            ClearField("currentEmotion");
            ClearField("currentPerson");
        }

        if (me.emotion === emotion) {
            me.currentEmotion = $('<img src="sprites/' + emotion + '.png" class="' + this.imageClass + '" />').appendTo(me.personContainer);
            me.currentPerson = $('<img src="sprites/' + me.person + '_' + emotion + '.png" class="' + this.imageClass + '" />').appendTo(me.personContainer);
        } else {
            me.oldEmotion = $('<img src="sprites/' + me.emotion + '.png" class="' + this.imageClass + '" />').appendTo(me.personContainer);
            me.currentEmotion = $('<img src="sprites/' + emotion + '.png" class="' + this.imageClass + ' fadein" />').appendTo(me.personContainer);
            me.oldPerson = $('<img src="sprites/' + me.person + '_' + me.emotion + '.png" class="' + this.imageClass + '" />').appendTo(me.personContainer);
            me.currentPerson = $('<img src="sprites/' + me.person + '_' + emotion + '.png" class="' + this.imageClass + ' fadein" />').appendTo(me.personContainer);
        }

        me.emotion = emotion;

        function ClearField(fieldName) {
            me[fieldName].remove();
            me[fieldName] = void 0;
        }
    }

    ShowBonus(text) {
        var bonus = $('<div class="attribute bonus ' + this.directionClass + '">' + text + '</div>');
        bonus.appendTo(this.personContainer);
        setTimeout(function () {
            bonus.addClass("out");
        }, 3000);
    }

    ShowPenalty(text) {
        var bonus = $('<div class="attribute penalty ' + this.directionClass + '">' + text + '</div>');
        bonus.appendTo(this.personContainer);
        setTimeout(function () {
            bonus.addClass("out");
        }, 3000);
    }
}

class LeftPersonBaloon extends PersonBaloon {
    constructor() {
        super("protag", "left", "");
        this.voiceArrowHtml = '<path d="M 15,0 L 15,28 L 0,28 z" />';
        this.thoughtArrowHtml = '<ellipse cx="14" cy="5" rx="4" ry="2" /><ellipse cx="11" cy="11" rx="6" ry="3" /><ellipse cx="8" cy="19" rx="8" ry="4" />';
    }
}

class RightPersonBaloon extends PersonBaloon {
    constructor() {
        super("interl", "right", "flipHorizontally");
        this.voiceArrowHtml = '<path d="M 0,0 L 15,28 L 0,28 z" />';
        this.thoughtArrowHtml = '<ellipse cx="6" cy="5" rx="4" ry="2" /><ellipse cx="9" cy="11" rx="6" ry="3" /><ellipse cx="12" cy="19" rx="8" ry="4" />';
    }
}

class TutorialBaloon extends Baloon {
    Show(container, title) {
        this.scene = $(
            '<div class="scene tutorial">' +
            '<div class="tutorialContainer">' +
            '<div class="baloon center">' +
            '<div class="baloonTitle">' +
            '<span>' + title + '</span>' +
            '</div>' +
            '<div class="baloonText withTitle"></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

        this.titleContainer = this.scene.find(".baloonTitle span");
        this.balloon = this.scene.find(".tutorialContainer").hide();
        this.textContainer = this.scene.find(".baloonText");
        this.scene.appendTo(container);
    }
}

class NarrationBaloon extends Baloon {
    Show(container) {
        this.scene = $(
            '<div class="scene tutorial">' +
            '<div class="tutorialContainer">' +
            '<div class="baloon center">' +
            '<div class="baloonText withoutTitle"></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

        this.balloon = this.scene.find(".tutorialContainer").hide();
        this.textContainer = this.scene.find(".baloonText");
        this.scene.appendTo(container);
    }
}