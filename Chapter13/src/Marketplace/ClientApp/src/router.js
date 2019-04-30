import Vue from "vue";
import Router from "vue-router";
import Home from "./views/Home";
import NewAd from "./views/NewAd";
import Login from "./views/Login";
import Register from "./views/Register";
import AdServices from "./views/AdServices";

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "home",
      component: Home
    },
    {
      path: "/new",
      name: "new",
      component: NewAd
    },
    {
      path: "/new/preview",
      name: "preview",
      component: AdServices
    },
    {
      path: "/login",
      name: "login",
      component: Login
    },
    {
      path: "/register",
      name: "register",
      component: Register
    },
    {
      path: "/about",
      name: "about",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ "./views/About")
    }
  ]
})
