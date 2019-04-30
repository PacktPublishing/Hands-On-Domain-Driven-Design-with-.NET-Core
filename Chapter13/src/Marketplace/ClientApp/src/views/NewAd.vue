<template>
    <v-layout row>
        <v-flex xs12 sm6 offset-sm3>
            <v-card>
                <v-card-title primary-title>
                    <div>
                        <div class="headline">New Classified Ad</div>
                        <span class="grey--text">Fill out the details and then click the Add button</span>
                    </div>
                </v-card-title>
                <v-card-text>
                    <v-form>
                        <ad-title v-bind:ad-title="title"/>
                        <ad-text v-bind:ad-text="text"/>
                        <ad-price v-bind:ad-price="price"/>
                        <ad-image/>
                        <v-btn color="primary" @click="add">Next</v-btn>
                        <v-btn to="/">Cancel</v-btn>
                    </v-form>
                </v-card-text>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import {mapGetters, mapActions} from "vuex";
    import {uuid} from "vue-uuid";
    import {CreateAd, DeleteAdIfEmpty} from "../store/modules/ads/actions.type";
    import AdTitle from "../components/AdTitle";
    import AdText from "../components/AdText";
    import AdPrice from "../components/AdPrice";
    import AdImage from "../components/AdImage";
    import store from "../store";

    export default {
        components: {
            AdTitle,
            AdText,
            AdPrice,
            AdImage
        },
        data: () => ({
            title: null,
            text: null,
            price: null,
            imageData: "",
            snackBar: {}
        }),
        computed: {
            ...mapGetters("ad", ["currentAd"])
        },
        methods: {
            add() {
                this.$router.push({name: "preview"});
            }
        },
        async beforeRouteEnter(to, from, next) {
            let adId = uuid.v1();
            await store.dispatch("ad/" + CreateAd, adId);
            return next();
        },
        async beforeRouteLeave(to, from, next) {
            await store.dispatch("ad/" + DeleteAdIfEmpty, this.currentAd.id);
            next();
        }
    }
</script>

<style scoped>

</style>