<script setup lang="ts">
import { inject } from 'vue';
import { ref } from 'vue'
import { type SearchApi } from '../api/SearchApi'

const searchApi: SearchApi = inject("SearchAPI") as SearchApi;

const loading = ref<boolean>(false);
// defineProps<{ msg: string }>()
const searchTerm = ref<string>("")
// const count = ref(0)
const onClick = async () => {
    console.log(searchTerm.value);
    if (searchTerm.value) {
        const resp = await searchApi.searchCoffees(searchTerm.value);
        console.log(resp);
    }
}
</script>

<template>
    <v-card-text>
        <v-text-field v-model="searchTerm" :loading="loading" density="compact" variant="solo" label="Search all"
            append-inner-icon="fas fa-search" single-line clearable @click:append-inner="onClick"></v-text-field>
    </v-card-text>
</template>

<style scoped></style>
