$(function() {
    var treeParams = window.the_nwazet_tree,
        serviceUrl = treeParams.serviceUrl,
        explorerUrl = treeParams.explorerUrl,
        res = treeParams.res,
        appendWaitNode = function (parent) {
            parent
                .children(".the-tree-leaf-label, .the-tree-root")
                .after(
                    $("<div></div>")
                        .addClass("the-tree-waits")
                );
        },
        populateBranch = function(parent) {
            $.ajax({
                url: serviceUrl
                    .replace("__id__", encodeURIComponent(parent.data("id")))
                    .replace("__type__", encodeURIComponent(parent.data("type"))),
                type: "POST",
                dataType: "json",
                success: function(data) {
                    parent
                        .data("children", data)
                        .hide()
                        .empty()
                        .parent()
                        .find(".the-tree-waits")
                        .remove();
                    $.each(data, function() {
                        var childElement = $("<li></li>")
                            .addClass("the-tree-leaf");
                        if (!this.IsLeaf) {
                            childElement.append(
                                $("<div></div>")
                                    .addClass("the-tree-expando-glyph")
                                    .append(
                                        $("<a></a>", {
                                            href: "#"
                                        })
                                            .click(function(e) {
                                                e.preventDefault();
                                                var expandoGlyph = $(this)
                                                    .parent()
                                                    .toggleClass("open");
                                                if (!expandoGlyph.data("already-open")) {
                                                    expandoGlyph.data("already-open", true);
                                                    appendWaitNode(childElement);
                                                    populateBranch(childBranch);
                                                } else {
                                                    childBranch.slideToggle();
                                                }
                                            })
                                    )
                            );
                        }
                        childElement.append(
                            (this.Url ? $("<a></a>", { href: this.Url }) : $("<span></span>"))
                                .addClass("the-tree-leaf-label the-tree-leaf-" + this.Type + (this.CssClass || ""))
                                .text(this.Title)
                        );
                        var childBranch = $("<ul></ul>")
                                .data({
                                    id: this.Id,
                                    type: this.Type
                                })
                                .addClass("the-tree-branch");
                        childElement.append(childBranch);
                        parent.append(childElement).slideDown("slow");
                    });
                },
                error: function() {
                    parent
                        .empty()
                        .append(
                            $("<li></li>")
                                .text(res.loadChildError)
                        );
                }
            });
        },
        neverOpen = true,
        openTag = $("<div></div>")
            .addClass("the-tree-open-tag")
            .append($("<div></div>")
                .append($("<a href=\"#\"></a>")
                    .text(res.openTagLabel)
                    .on("click", function(e) {
                        e.preventDefault();
                        if (neverOpen) {
                            appendWaitNode(theTreeContainer);
                            treeRoot.toggle("slow");
                            populateBranch(treeRoot);
                            neverOpen = false;
                        }
                        else {
                            treeRoot.toggle("slow");
                        }
                        openTag.toggleClass("open");
                    }))
            ),
        treeRoot = $("<ul></ul>")
            .addClass("the-tree-root")
            .data({
                id: "/",
                type: "root"
            })
            .hide(),
        theTreeContainer = $("<div></div>")
            .addClass("the-tree-container")
            .append(openTag)
            .append(treeRoot),
        theTreeExplorer = $(".the-tree-explorer-panes");

    $(".the-tree-explorer-panes")
        .on("click", ".the-tree-explorer-list li", function(e) {
            var target = $(e.target), expandedList, listItem,
                contentZone = $(".nwazet-tree .zone-content"),
                expandedPane = $("<li class=\"the-tree-explorer-pane\"></li>")
                    .append(expandedList = $("<ul class=\"the-tree-explorer-list\"><li style=\"padding-top:0\"><div class=\"the-explorer-waits\"></div></li></ul>")),
                currentNode = target.closest("li");
            if (target.hasClass("the-tree-explorer-node") && target.attr("href")) return;
            if (!target.hasClass("the-tree-explorer-expand-node")) target = target.closest("li").find(".the-tree-explorer-expand-node");
            currentNode.siblings().removeClass("selected");
            currentNode.addClass("selected").parent().closest("li").nextAll().remove();
            theTreeExplorer.append(expandedPane);
            $.ajax({
                url: target.attr("href"),
                type: "POST",
                dataType: "json",
                success: function (data) {
                    expandedList.empty();
                    $.each(data.Children, function() {
                        expandedList.append(
                            listItem = $("<li></li>")
                                .append($("<a class=\"the-tree-explorer-node\"></a>")
                                    .attr("href", this.Url)
                                    .text(this.Title))
                        );
                        if (!this.IsLeaf) {
                            listItem.append(
                                $("<a class=\"the-tree-explorer-expand-node\">&gt;</a>")
                                    .attr("href", explorerUrl
                                        .replace("__id__", encodeURIComponent(this.Id))
                                        .replace("__type__", encodeURIComponent(this.Type)))
                            );
                        }
                    });
                    var fullWidth = theTreeExplorer.width(),
                        availableWidth = contentZone.width();
                    contentZone.animate({
                        scrollLeft: fullWidth - availableWidth - Math.max(expandedPane.width() - availableWidth, 0)
                    }, "fast");
                }
            });
            e.preventDefault();
        });
    
    $("body").append(theTreeContainer);
});