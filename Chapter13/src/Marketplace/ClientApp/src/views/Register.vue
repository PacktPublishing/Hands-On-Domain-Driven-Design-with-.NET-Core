<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <v-card class="elevation-12">
                    <v-toolbar dark color="primary">
                        <v-toolbar-title>Register</v-toolbar-title>
                    </v-toolbar>
                    <v-card-text>
                        <v-form>
                            <v-text-field
                                    name="login"
                                    label="Login"
                                    type="text"
                                    v-model="userName"/>
                            <v-text-field
                                    name="fullName"
                                    label="Full name"
                                    type="text"
                                    v-model="fullName"/>
                            <v-text-field
                                    id="password"
                                    name="password"
                                    label="Password"
                                    type="password"
                                    v-model="password"/>
                        </v-form>
                        <div v-if="errors" class="error">
                            <v-alert v-for="error in errors" :value="true" :key="error" type="error">
                                {{ error }}
                            </v-alert>
                        </div>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn flat color="secondary" :to="{ name: 'login' }">Login</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" v-on:click="signUp">Sign up</v-btn>
                    </v-card-actions>
                </v-card>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
    import {mapState} from "vuex";
    import {Register} from "../store/modules/auth/actions.type";

    export default {
        name: "",
        data: () => ({
            userName: null,
            fullName: null,
            password: null,
        }),
        methods: {
            async signUp() {
                let user = {
                    userName: this.userName, 
                    fullName: this.fullName,
                    password: this.password
                };
                await this.$store.dispatch(Register, user);
            }
        },
        computed: {
            ...mapState({
                errors: state => state.auth.errors
            })
        }
    }
</script>
