<template>
    <v-app>
        <Header></Header>
        <v-content>
            <v-container fluid>
                <router-view></router-view>
            </v-container>
        </v-content>
    </v-app>
</template>

<script>
    import Header from "./components/Header";
    
    export default {
        name: "App",
        components: {
            Header
        }
    }
</script>
