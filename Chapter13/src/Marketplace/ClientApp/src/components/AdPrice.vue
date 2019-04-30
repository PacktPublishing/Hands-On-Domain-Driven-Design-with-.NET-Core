<template>
    <v-text-field
            v-model="price"
            label="Price"
            required
            :error-messages="validatePrice"
            @blur="$v.price.$touch(); updatePrice();"
    />
</template>

<script>
    import {mapActions} from "vuex";
    import {validationMixin} from "vuelidate";
    import {required, numeric} from "vuelidate/lib/validators";
    import {UpdateAdPrice} from "../store/modules/ads/actions.type";

    export default {
        name: "",
        mixins: [validationMixin],
        props: {
            adPrice: Number
        },
        validations: {
            price: {required, numeric}
        },
        data: function () {
            return {
                price: this.adPrice
            }
        },
        computed: {
            validatePrice() {
                const errors = [];
                if (!this.$v.price.$dirty) return errors;
                !this.$v.price.numeric && errors.push("Price must be a valid number");
                !this.$v.price.required && errors.push("Ad must have a price");
                return errors;
            },
        },
        methods: {
            async updatePrice() {
                if (this.validatePrice.length > 0) return;
                try {
                    await this.updateAdPrice(this.price);
                } catch (e) {
                    console.log(JSON.stringify(e));
                }
            },
            ...mapActions("ad", {
                updateAdPrice: UpdateAdPrice
            })
        },
    }
</script>

<style scoped>

</style>