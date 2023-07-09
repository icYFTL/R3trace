export const general = {
    namespaced: true,
    state: {
        menuEnabled: true,
        headerEnabled: true,
        sidebarVisible: false,
        sidebarUnfoldable: false
    },
    getters: {
        menuState: state => state.menuEnabled,
        headerState: state => state.headerEnabled,
        sidebarState: state => state.sidebarVisible
    },
    actions: {
        setMenuVisibility({commit}, visibility) {
            commit('setMenuVisibility', visibility);
        },
        setHeaderVisibility({commit}, visibility) {
            commit('setHeaderVisibility', visibility);
        },

    },
    mutations: {
        setMenuVisibility(state, visibility){
            state.menuEnabled = visibility;
        },
        setHeaderVisibility(state, visibility){
            state.headerEnabled = visibility;
        },
        toggleSidebar(state) {
            state.sidebarVisible = !state.sidebarVisible
        },
        toggleUnfoldable(state) {
            state.sidebarUnfoldable = !state.sidebarUnfoldable
        },
        updateSidebarVisible(state, payload) {
            state.sidebarVisible = payload.value
        },
    }
}
