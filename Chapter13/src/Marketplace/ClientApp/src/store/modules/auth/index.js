import ApiService from "../../../common/api.service";
import {
    CheckAuth,
    Login,
    Register
} from "./actions.type";
import {
    Authorized, 
    ErrorOccured, 
    Unauthorized,
    UserDetailsReceived
} from "./mutations.type";

const state = {
    errors: null,
    user: {},
    isAuthenticated: false
};

const getters = {
    currentUser(state) {
        return state.user;
    },
    isAuthenticated(state) {
        return state.isAuthenticated;
    }
};

const actions = {
    async [Login](context, credentials) {
        try {
            await ApiService.post("/auth/login", credentials);
            context.commit(Authorized);
            let response = await ApiService.get("/profile", credentials.password);
            context.commit(UserDetailsReceived, response.data);
        } catch (error) {
            if (error.response.status === 401){
                context.commit(ErrorOccured, { error: "Invalid user name or password" });
            } else {
                context.commit(ErrorOccured, { error: "Error occured on the server" });
            }
            throw error;
        }
    },
    [CheckAuth](context) {
        context.commit(ErrorOccured, null);
    },
    async [Register](context, user) {
        try {
            let request = {
                userId: user.password,
                displayName: user.userName,
                fullName: user.fullName
            };
            await ApiService.post("/profile", request);
            await context.dispatch(Login, user);
        } catch (error) {
            context.commit(ErrorOccured, { error: "Error occured on the server" });
        }
    }
};

const mutations = {
    [ErrorOccured](state, error) {
        state.errors = error;
    },
    [Authorized](state) {
        state.isAuthenticated = true;
        state.errors = {};
    },
    [Unauthorized](state) {
        state.isAuthenticated = false;
        state.user = {};
        state.errors = {};
    },
    [UserDetailsReceived](state, user) {
        state.user = user;
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}