<template xmlns:v-slot="http://www.w3.org/1999/XSL/Transform">
    <v-toolbar app color="primary">
        <v-btn icon :to="{ name: 'home' }">
            <v-icon>home</v-icon>
        </v-btn>
        <v-toolbar-title class="headline text-uppercase">
            <span>Marketplace</span>
        </v-toolbar-title>

        <v-spacer></v-spacer>

        <v-btn v-if="!isAuthenticated" color="primary darken-1" :to="{ name: 'login' }">
            Sign-in
        </v-btn>
        <div v-else>
            <span class="headline">{{ currentUser.displayName }}</span>
            <NewAdButton/>
            <v-menu bottom left>
                <template v-slot:activator="{ on }">
                    <v-btn v-on="on" color="primary darken-1" icon>
                        <v-icon>more_vert</v-icon>
                    </v-btn>
                </template>
                <v-list>
                    <v-list-tile @click="">
                        <v-list-tile-title>Your ads</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="">
                        <v-list-tile-title>Your orders</v-list-tile-title>
                    </v-list-tile>
                </v-list>
            </v-menu>
        </div>
    </v-toolbar>
</template>

<script>
    import NewAdButton from "../components/NewAdButton";
    import {mapGetters} from "vuex";

    export default {
        name: "",
        components: {
            NewAdButton
        },
        computed: {
            ...mapGetters(["currentUser", "isAuthenticated"])
        },
        data: () => ({
            items: [
                {title: 'Click Me'},
                {title: 'Click Me'},
                {title: 'Click Me'},
                {title: 'Click Me 2'}
            ]
        })
    };
</script>

<style scoped>

</style>