let regexGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

let htmlEncodeObj = function (model) {
    let obj = angular.copy(model);
    Object.keys(obj).forEach(key => {
        if (typeof obj[key] === 'object' && obj[key] !== null) {
            htmlEncodeObj(obj[key]);
        }
        else if (typeof obj[key] === 'string' && obj[key] !== null) {
            obj[key] = angular.element('<pre/>').text(obj[key]).html();
        }
    });
    return obj;
};

const ucUtils = {
    htmlEncodeObj: htmlEncodeObj,
    getSettings: function (key) {
        if (!Umbraco || !Umbraco.Sys || !Umbraco.Sys.ServerVariables || !Umbraco.Sys.ServerVariables['umbracoCommerce'] || !Umbraco.Sys.ServerVariables['umbracoCommerce'][key]) {
            throw 'No Umbraco Commerce setting found with key ' + key;
        }
        return Umbraco.Sys.ServerVariables['umbracoCommerce'][key];
    },
    parseCompositeId: function (id) {
        return id.replace(/[‐᠆﹣－⁃−]+/gi, '-').split('_');
    },
    createCompositeId: function (ids) {
        return ids.join('_');
    },
    createSettingsBreadcrumbFromTreeNode: function (treeNode) {
        let breadcrumb = [];

        let currentNode = treeNode;
        while (currentNode.nodeType !== 'Stores' && currentNode.level > 2) {
            breadcrumb.splice(0, 0, {
                name: currentNode.name,
                routePath: currentNode.routePath,
            });
            currentNode = currentNode.parent();
        }

        return breadcrumb;
    },
    createBreadcrumbFromTreeNode: function (treeNode) {

        console.log(treeNode);
        let breadcrumb = [];

        let currentNode = treeNode;
        while (currentNode.level > 0) {
            breadcrumb.splice(0, 0, {
                name: currentNode.name,
                routePath: currentNode.routePath,
            });
            currentNode = currentNode.parent();
        }

        return breadcrumb;
    },
    generateGuid: function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    },
    isGuid: function (stringToTest) {
        if (stringToTest[0] === '{') {
            stringToTest = stringToTest.substring(1, stringToTest.length - 1);
        }
        return regexGuid.test(stringToTest);
    },
};

export default ucUtils;