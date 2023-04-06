import { defineStore } from 'pinia'
import { Ref, inject, ref } from 'vue';
import { SearchApi } from '../api/SearchApi';
import { watch } from 'vue';


export const useSearchStore = defineStore('searches', () => {
    const searchApi: SearchApi = inject("SearchAPI") as SearchApi;
    
    const _loading = ref(false);
    const _currentSearchTerm = ref('');
    const _currentSearchResults = ref<any>();

    
    async function searchAll() {
        const searchTerm = _currentSearchTerm.value;
        if (searchTerm) {
            _loading.value = true;
            _currentSearchResults.value = await searchApi.searchAll(searchTerm);
            _loading.value = false;
        }
    }

    const loading = (): Ref<boolean> =>{
        return _loading;
    }
    const currentSearchResults = (): Ref<any> =>{
        return _currentSearchResults;
    }
    const currentSearchTerm = (): Ref<string> =>{
        return _currentSearchTerm;
    }

    watch(_currentSearchTerm, searchAll);
    
    const setCurrentSearchTerm = (searchTerm:string): void =>{
        _currentSearchTerm.value = searchTerm;
    }
    return {
        loading,
        currentSearchResults,
        currentSearchTerm,
        setCurrentSearchTerm
    }

})