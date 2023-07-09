import { createStore } from "vuex";
import { auth } from "./auth.module";
import {general} from "./general.module";

const store = createStore({
    modules: {
        general,
        auth
    },
});
export default store;
