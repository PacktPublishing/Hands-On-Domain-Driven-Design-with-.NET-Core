<template xmlns:v-slot="http://www.w3.org/1999/XSL/Transform">
    <v-layout row wrap>
        <v-flex xs6>
            <v-image-input
                    v-model="imageData"
                    :image-quality="0.85"
                    clearable
                    image-format="png"
            ></v-image-input>
            <v-btn
                    color="primary"
                    :disabled="!imageData"
                    @click="uploadImage">
                Upload
            </v-btn>
        </v-flex>
        <v-flex xs6>
            <v-container
                    fluid
                    grid-list-lg
                    fill-height
                    style="min-height: 434px"
            >
                <v-fade-transition mode="out-in">
                    <v-layout
                            wrap
                            v-for="image in currentAd.images"
                            :key="image.key"
                    >
                        <v-flex xs4>
                            <v-card flat>
                                <img
                                        :src="image.image"
                                        class="grey lighten-2"
                                >
                            </v-card>
                        </v-flex>
                    </v-layout>
                </v-fade-transition>
            </v-container>
        </v-flex>
    </v-layout>
</template>

<script>
    import VImageInput from 'vuetify-image-input';
    import {mapActions, mapGetters} from "vuex";
    import {UploadAdImage} from "../store/modules/ads/actions.type";

    export default {
        name: "",
        components: {
            [VImageInput.name]: VImageInput
        },
        data: function () {
            return {
                imageData: null,
            }
        },
        computed: {
            ...mapGetters("ad", {
                currentAd: "currentAd"
            }),
        },
        methods: {
            async uploadImage() {
                await this.uploadAdImage(this.imageData);
            },
            ...mapActions("ad", {
                uploadAdImage: UploadAdImage
            })
        }
    }
</script>

<style scoped>

</style>