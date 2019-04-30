import ApiService from "../../../common/api.service";
import {
    FetchServices
} from "./actions.type";
import {
    ServicesDataReceived
} from "./mutations.type";

const state = {
    services: {}
};

const getters = {
    availableServices: state => state.services
};

const actions = {
    async [FetchServices](context) {
        let services = await ApiService.get("/services");
        context.commit(ServicesDataReceived, services.data);
    }
};

const mutations = {
    [ServicesDataReceived](state, services) {
        state.services = services;
    }
};

export default {
    state,
    getters,
    actions,
    mutations,
    namespaced: true
}
