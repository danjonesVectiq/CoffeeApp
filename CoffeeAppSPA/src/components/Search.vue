<script setup lang="ts">
import { inject, watch } from 'vue';
import { ref } from 'vue'
import { type SearchApi } from '../api/SearchApi'
import _debounce from 'lodash.debounce';
import { DebouncedFunc } from 'lodash';
import { onMounted } from 'vue';

const searchApi: SearchApi = inject("SearchAPI") as SearchApi;


const loading = ref<boolean>(false);
// defineProps<{ msg: string }>()
const searchTerm = ref<string>("")
// const count = ref(0)
const onClick = () => {
    abortDebounceFlag.cancel();
    searchAll();
}
let abortDebounceFlag = _debounce(searchAll, 300);


function searchAll() {
    if (searchTerm.value) {
        console.log(searchTerm.value);
        //     const resp = await searchApi.searchCoffees(searchTerm.value);
        //     console.log(resp);
    }
}
watch(searchTerm, abortDebounceFlag);


</script>

<template>
    <v-card-text>
        <v-text-field v-model="searchTerm" :loading="loading" density="compact" variant="underlined" label="Search all"
            append-icon="fas fa-search" single-line clearable @click:append="onClick"></v-text-field>
    </v-card-text>
</template>

<style scoped></style>
