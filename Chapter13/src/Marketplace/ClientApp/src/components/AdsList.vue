<template>
    <v-layout justify-center>
        <v-flex xs12>
            <v-container>
                <v-layout text-xs-center wrap>
                    <v-flex mb-4>
                        <h1 class="display-2 font-weight-bold mb-3">
                            Welcome to Marketplace
                        </h1>
                        <p class="subheading font-weight-regular">
                            There are no published ads to see<br>
                            <router-link v-if="isAuthenticated"
                                         :to="{name: 'new'}">
                                Publish your first ad now!
                            </router-link>
                            <router-link v-else :to="{name: 'register'}">
                                Register and
                                publish your first ad now!
                            </router-link>
                        </p>
                    </v-flex>
                </v-layout>
            </v-container>
            <v-container grid-list-md>
                <v-layout row wrap>
                    <v-flex
                            v-for="card in cards"
                            :key="card.title"
                            v-bind="{ [`xs${card.flex}`]: true }"
                    >
                        <AdListItem
                                :image="card.src"
                                :title="card.title"
                                :price="card.price"/>
                    </v-flex>
                </v-layout>
            </v-container>
        </v-flex>
    </v-layout>
</template>

<script>
    import {mapGetters} from "vuex";
    import AdListItem from "../components/AdListItem";

    export default {
        name: "",
        components: {
            AdListItem
        },
        data: () => ({
            cards: [
                {
                    title: 'Pre-fab homes',
                    src: 'https://cdn.vuetifyjs.com/images/cards/house.jpg',
                    price: "100",
                    flex: 6
                },
                {
                    title: 'Favorite road trips',
                    src: 'https://cdn.vuetifyjs.com/images/cards/road.jpg',
                    price: "200",
                    flex: 3
                },
                {
                    title: 'Best airlines',
                    src: 'https://cdn.vuetifyjs.com/images/cards/plane.jpg',
                    price: "150",
                    flex: 3
                }
            ]
        }),
        computed: {
            ...mapGetters(["isAuthenticated"])
        }
    };
</script>

<style scoped>

</style>