import { defineStore } from 'pinia'
import { Ref, inject, ref } from 'vue';
import { SearchApi } from '../api/SearchApi';
import { watch } from 'vue';


export const useSearchStore = defineStore('searches', () => {
    const searchApi: SearchApi = inject("SearchAPI") as SearchApi;
    
    const _loading = ref(false);
    const _currentSearchTerm = ref('');
    const _allSearchResults = ref<any[]>([]);

    
    async function searchAll() {
        const searchTerm = _currentSearchTerm.value;
        if (searchTerm) {
            _loading.value = true;
            const allResult = await searchApi.searchAll(searchTerm);
            _allSearchResults.value.splice(0);
            Object.values(allResult).forEach((results)=>{
                console.log(results);
                _allSearchResults.value.push(...results);
            })
            _loading.value = false;
        }
    }

    const loading = (): Ref<boolean> =>{
        return _loading;
    }
    const allSearchResults = (): Ref<any[]> =>{
        return _allSearchResults;
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
        allSearchResults,
        currentSearchTerm,
        setCurrentSearchTerm
    }

})