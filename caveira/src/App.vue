<template>
    <div class="app">
        <HeaderComponent/>
        <div class="module">
            <router-view/>
        </div>
    </div>
</template>

<script>

import HeaderComponent from "@/components/header/HeaderComponent.vue";

export default {
    name: 'App',
    components: {HeaderComponent}
}
</script>

<style scoped>
.app {
    height: 100vh;
}

.module {
    width: 100%;
    height: 100%;
    top: 0;
}
</style>
