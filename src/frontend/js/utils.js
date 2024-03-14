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

    /**
     * 
     * @param {Function} func 
     * @param {number} timeout in milliseconds
     * @returns A debounced function.
     */
    debounce: function (func, timeout = 300) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => { func.apply(this, args); }, timeout);
        };
    },
};

export default ucUtils;