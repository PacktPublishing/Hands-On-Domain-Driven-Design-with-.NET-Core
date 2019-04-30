import Vue from "vue"
import Vuex from "vuex"

import auth from "./modules/auth";
import ad from "./modules/ads";
import services from "./modules/services";

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        ad,
        services
    }
})
