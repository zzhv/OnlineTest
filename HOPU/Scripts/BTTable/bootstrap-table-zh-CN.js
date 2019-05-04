/**
 * Bootstrap Table Chinese translation
 * Author: Zhixin Wen<wenzhixin2010@gmail.com>
 */
($ => {
    $.fn.bootstrapTable.locales['zh-CN'] = {
        formatLoadingMessage() {
            return '<span class="btn btn-success">正在努力地加载数据中，请稍候…… </span>'
        },
        formatRecordsPerPage(pageNumber) {
            return `每页显示 ${pageNumber} 条记录`
        },
        formatShowingRows(pageFrom, pageTo, totalRows) {
            return `当前为第<span class="btn btn-info"> ${pageFrom} </span>到第<span class="btn btn-info"> ${pageTo} </span>条记录，总共<span class="btn btn-info"> ${totalRows} </span>条记录`
        },
        formatSearch() {
            return '多条件搜索请用逗号隔开'
        },
        formatNoMatches() {
            return '<span class="btn btn-danger">没有找到匹配的信息,请重试！</span>'
        },
        formatPaginationSwitch() {
            return '隐藏/显示分页'
        },
        formatRefresh() {
            return '刷新'
        },
        formatToggle() {
            return '切换视图'
        },
        formatColumns() {
            return '列'
        },
        formatExport() {
            return '导出数据'
        },
        formatClearFilters() {
            return '清空过滤'
        }
    }

    $.extend($.fn.bootstrapTable.defaults, $.fn.bootstrapTable.locales['zh-CN'])
})(jQuery)
