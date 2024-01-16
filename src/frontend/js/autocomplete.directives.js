import { MODULE } from './constants';
import ucUtils from './utils';

angular.module(MODULE.NAME, []).component('autocomplete', function Autocomplete() {
    const vm = this;
    vm.searchText = '';
    vm.suggestions = [];

    const getSuggestionsAsync = async (searchText) => {
        console.log('searching: ' + searchText);
    };

    vm.onSearchTextChange = ucUtils.debounce(getSuggestionsAsync(vm.searchText));
});