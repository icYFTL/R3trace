import {createApp} from 'vue'
import App from './App.vue'
import router from "./router";
import store from "./store";
import '@coreui/coreui/dist/css/coreui.css';
import "bootstrap";
import "bootstrap/dist/css/bootstrap.min.css";
// import {FontAwesomeIcon} from './plugins/font-awesome';
import CIcon from '@coreui/icons-vue'
import {iconsSet as icons} from '@/assets/icons'
// import {SetupCalendar} from 'v-calendar';
// import 'v-calendar/dist/style.css';
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
import "@/assets/css/custom-toast.css"
import PrimeVue from "primevue/config";

import 'primevue/resources/themes/saga-blue/theme.css';
import 'primevue/resources/primevue.min.css';
// import 'primeicons/primeicons.css';
// import 'primeflex/primeflex.css'


createApp(App)
    // .use(SetupCalendar, {})
    .use(store)
    .use(router)
    .use(Toast)
    .component('CIcon', CIcon)
    .provide('icons', icons)
    // .use(PrimeVue, {
    //     zIndex: {
    //         modal: 1100,        //dialog, sidebar
    //         overlay: 9999,      //dropdown, overlaypanel
    //         menu: 1000,         //overlay menus
    //         tooltip: 1100       //tooltip
    //     }
    // })
    // .component('CIcon', CIcon)
    // .provide('icons', icons)
    // .component("font-awesome-icon", FontAwesomeIcon)
    .mount('#app')

