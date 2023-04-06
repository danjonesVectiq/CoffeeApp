<script setup lang="ts">
import { watch } from 'vue';
import { ref } from 'vue'
import _debounce from 'lodash.debounce';
import { useSearchStore } from '../stores/searchStore';


const searchStore = useSearchStore();
// const loading = ref<boolean>(false);
// defineProps<{ msg: string }>()
const loading = searchStore.loading();
const searchTerm = ref<string>("")
// const count = ref(0)
const onClick = () => {
    abortDebounceFlag.cancel();
    searchAll();
}
let abortDebounceFlag = _debounce(searchAll, 300);


function searchAll() {
    searchStore.setCurrentSearchTerm(searchTerm.value);
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
