<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <v-card class="elevation-12">
                    <v-toolbar dark color="primary">
                        <v-toolbar-title>Login</v-toolbar-title>
                    </v-toolbar>
                    <v-card-text>
                        <v-form>
                            <v-text-field
                                    prepend-icon="person"
                                    name="login"
                                    label="Login"
                                    type="text"
                                    v-model="userName"/>
                            <v-text-field
                                    id="password"
                                    prepend-icon="lock"
                                    name="password"
                                    label="Password"
                                    type="password"
                                    v-model="password"/>
                        </v-form>
                        <div v-if="errors" class="error">
                            <v-alert v-for="error in errors" :value="true" type="error">
                                {{ error }}
                            </v-alert>
                        </div>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn flat color="secondary" :to="{ name: 'register' }">Sign up</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" v-on:click="login">Login</v-btn>
                    </v-card-actions>
                </v-card>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
    import {mapState} from "vuex";
    import {Login} from "../store/modules/auth/actions.type";

    export default {
        name: "",
        data: () => ({
            userName: null,
            password: null
        }),
        methods: {
            async login() {
                let credentials = {userName: this.userName, password: this.password};
                try {
                    await this.$store.dispatch(Login, credentials);
                    this.$router.push({name: "home"});
                } catch { }
            }
        },
        computed: {
            ...mapState({
                errors: state => state.auth.errors
            })
        }
    }
</script>
