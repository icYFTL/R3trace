import {createWebHistory, createRouter} from "vue-router";
import LoginView from "@/views/LoginView.vue";
import RegisterView from "@/views/RegisterView.vue";

// lazy-loaded
// const Profile = () => import("./components/Profile.vue")
// const BoardAdmin = () => import("./components/BoardAdmin.vue")
// const BoardModerator = () => import("./components/BoardModerator.vue")
// const BoardUser = () => import("./components/BoardUser.vue")
const routes = [
    // {
    //     path: "/",
    //     component: Dashboard,
    //     name: 'Nova | Dashboard'
    // },
    // {
    //     path: "/dashboard",
    //     component: Dashboard,
    //     name: 'Nova | Dashboard'
    // },
    // {
    //     path: "/services",
    //     component: ServicesView,
    //     name: 'Nova | Services'
    // },
    {
        path: "/login",
        component: LoginView,
        name: 'R3Trace | Login'
    },
    {
        path: "/register",
        component: RegisterView,
        name: 'R3Trace | Sign up'
    },
    // {
    //     path: "/settings",
    //     component: SettingsView,
    //     name: 'Nova | Settings'
    // },
    // {
    //     path: "/services/vk",
    //     component: VkServiceComponent,
    //     name: 'Nova | Services>Vk'
    // }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});
const publicPages = ['/login', '/register'];
// const store = require('./store/index');

router.beforeEach((to, from, next) => {
    const authRequired = !publicPages.includes(to.path);
    const loggedIn = localStorage.getItem('token');
    // if (to.path !== '/logout' && to.path !== '/login')
    //     store.default.dispatch('auth/checkToken');

    if (authRequired && !loggedIn) {
        next('/login');
    } else {
        document.title = to.name;
        next();
    }

});
//TODO: Fix

export default router;
