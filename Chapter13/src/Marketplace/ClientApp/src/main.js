import Vue from "vue";
import "./plugins/vuetify";
import VueLodash from 'vue-lodash'
import App from "./App.vue";
import router from "./router";
import store from "./store";
import UUID from "vue-uuid";
import Vuelidate from "vuelidate";
import "roboto-fontface/css/roboto/roboto-fontface.css";
import "material-design-icons-iconfont/dist/material-design-icons.css";
import {CheckAuth} from "./store/modules/auth/actions.type";
import ApiService from "./common/api.service";

Vue.config.productionTip = false;
Vue.use(UUID);
Vue.use(Vuelidate);
Vue.use(VueLodash);

ApiService.init();

router.beforeEach(async (to, from, next) => {
    await store.dispatch(CheckAuth);
    next();
});

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app');
