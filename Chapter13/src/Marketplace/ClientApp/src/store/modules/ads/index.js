import ApiService from "../../../common/api.service";
import {
    CreateAd, DeleteAdIfEmpty,
    RenameAd,
    UpdateAdPrice,
    UpdateAdText, UploadAdImage
} from "./actions.type";
import {
    AdCreated, AdImageAdded,
    AdPriceUpdated,
    AdRenamed,
    AdTextUpdated,
    CurrentAdCleared
} from "./mutations.type";

const state = {
    errors: null,
    ad: {
        images: []
    },
    notification: {}
};

const getters = {
    currentAd: state => state.ad,
    adNotification: state => state.notification
};

const actions = {
    async [CreateAd](context, uuid) {
        await ApiService.post("/ad", {id: uuid});
        context.commit(AdCreated, uuid);
    },
    async [RenameAd](context, title) {
        if (context.state.ad && title === context.state.ad.title) return;
        await ApiService.put(
            "/ad/title",
            {id: context.state.ad.id, title: title});
        context.commit(AdRenamed, title);
    },
    async [UpdateAdText](context, text) {
        if (text === context.state.ad.text) return;
        await ApiService.put(
            "/ad/text",
            {id: context.state.ad.id, text: text});
        context.commit(AdTextUpdated, text);
    },
    async [UpdateAdPrice](context, price) {
        if (price === context.state.ad.price) return;
        await ApiService.put(
            "/ad/price",
            {id: context.state.ad.id, price: price});
        context.commit(AdPriceUpdated, price);
    },
    async [DeleteAdIfEmpty](context, adId) {
        let {id, ...content} = context.state.ad;
        if (adId !== id) return;
        if (Object.keys(content).length === 0) {
            await ApiService.post("/ad/delete", {id});
        }
    },
    async [UploadAdImage](context, imageData) {
        await ApiService.post("/ad/image",
            {id: context.state.ad.id, image: imageData}
        );
        context.commit(AdImageAdded, imageData);
    }
};

const mutations = {
    [AdCreated](state, id) {
        state.ad = {id: id};
    },
    [AdRenamed](state, title) {
        state.ad.title = title;
    },
    [AdTextUpdated](state, text) {
        state.ad.text = text;
    },
    [AdPriceUpdated](state, price) {
        state.ad.price = price;
    },
    [CurrentAdCleared](state) {
        state.ad = {};
    },
    [AdImageAdded](state, image) {
        if (!state.ad.images) {
            state.ad.images = [{key: 0, image: image}];
        } else {
            state.ad.images.push({key: state.ad.images.length, image: image});
        }

    }
};

export default {
    state,
    getters,
    actions,
    mutations,
    namespaced: true
}
