<template>
    <v-flex v-bind="{ [`xs${flex}`]: true }">
        <v-card :dark="dark" :elevation="elevation">
            <v-img
                    :src="image"
                    height="200px"
            >
                <v-container
                        fill-height
                        fluid
                        pa-2
                >
                    <v-layout fill-height>
                        <v-flex xs12 align-end flexbox>
                        <span class="headline white--text"
                              v-text="'€' + price"></span>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-img>
            <v-card-title primary-title>
                <div>
                    <div class="headline">{{ displayTitle }}</div>
                    <!--                <span class="grey&#45;&#45;text">{{ price }}</span>-->
                </div>
            </v-card-title>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn icon>
                    <v-icon>favorite</v-icon>
                </v-btn>
                <v-btn icon>
                    <v-icon>bookmark</v-icon>
                </v-btn>
                <v-btn icon>
                    <v-icon>share</v-icon>
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-flex>
</template>

<script>
    export default {
        name: "",
        props: {
            image: String,
            title: String,
            price: String,
            size: Number,
            dark: Boolean,
            elevation: Number
        },
        computed: {
            displayTitle() {
                let title = this.title.slice(0, this.size * 4);
                if (title.length < this.title.length)
                    title += "...";
                return title;
            },
            flex() {
                return this.size * 1;
            }
        }
    }
</script>

<style scoped>

</style>