<template>
    <CHeader position="sticky" class="mb-0">
        <CContainer fluid>
            <CHeaderToggler class="ps-1" @click="toggleSideBar">
                <CIcon icon="cil-menu" size="lg"/>
            </CHeaderToggler>
            <!--      <CHeaderBrand class="mx-auto d-lg-none" to="/">
                    <CIcon :icon="logo" height="48" alt="Logo" />
                  </CHeaderBrand>-->
            <CHeaderNav class="d-none d-md-flex me-auto">
                <CNavItem>
                    <CNavLink href="/dashboard"> Dashboard</CNavLink>
                </CNavItem>
                <CNavItem>
                    <CNavLink href="/services">Services</CNavLink>
                </CNavItem>
                <CNavItem>
                    <CNavLink href="/settings">Settings</CNavLink>
                </CNavItem>
            </CHeaderNav>
            <CHeaderNav>
                <CNavItem v-if="!this.isLoggedIn">
                    <div class="d-flex flex-row">
                        <CNavLink href="/login">SignIn</CNavLink>
                        <CNavLink href="/register">SignUp</CNavLink>
                    </div>
                </CNavItem>
                <CNavItem v-else>
                    <CNavLink v-on:click="logout" href="#">Logout</CNavLink>
                </CNavItem>
            </CHeaderNav>
        </CContainer>
    </CHeader>

    <Sidebar v-model:visible="visible">
        <div class="sidebar-header">
            <div class="sidebar-avatar">
                <img src="@/assets/avatar.png" alt="avatar"/>
            </div>
            <div class="sidebar-user">Hello, {{ username }}</div>
        </div>
        <nav class="sidebar-menu">
            <ul>
                <li>
                    <router-link to="/dashboard">
                        <i class="pi pi-chart-line"></i>
                        <span>Dashboard</span>
                    </router-link>
                </li>
                <li>
                    <i class="pi pi-cog"></i>
                    <span>Settings</span>
                    <ul>
                        <li>
                            <router-link to="/vkcore">VkCore</router-link>
                        </li>
                        <li>
                            <router-link to="/general">General</router-link>
                        </li>
                    </ul>
                </li>
                <li>
                    <a href="#" @click="logout">
                        <i class="pi pi-power-off"></i>
                        <span>Logout</span>
                    </a>
                </li>
            </ul>
        </nav>
    </Sidebar>
</template>

<script>
import {CHeader, CHeaderNav, CContainer, CNavLink, CNavItem, CHeaderToggler} from '@coreui/vue'
import Sidebar from 'primevue/sidebar';
import {mapGetters} from 'vuex'
import {CIcon} from "@coreui/icons-vue";


export default {
    name: "HeaderComponent",
    components: {
        CHeader, CHeaderNav, CContainer, CNavLink, CNavItem, CHeaderToggler, Sidebar, CIcon
    },
    data() {
        return {
            visible: false,
            username: "John Doe"
        }
    },
    methods: {
        toggleSideBar() {
            this.visible = !this.visible;
        },
        logout() {
            this.$store.dispatch("auth/logout").then(
                () => {
                    this.$router.push("/login");
                },
                (error) => {
                    console.log(error)
                }
            );
        }
    },
    computed: {
        ...mapGetters('general', ['sidebarState']),
        sidebarVisible() {
            return this.slidebarState
        },
        isLoggedIn() {
            return this.$store.getters.isLoggedIn
        }
    },
}
</script>

<style scoped>
.sidebar-header {
    display: flex;
    align-items: center;
    padding: 1rem;
    border-bottom: 1px solid #eaeaea;
}

.sidebar-avatar {
    width: 3rem;
    height: 3rem;
    border-radius: 50%;
    overflow: hidden;
    margin-right: 1rem;
}

.sidebar-avatar img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.sidebar-user {
    font-weight: bold;
    font-size: 1.2rem;
}

.sidebar-menu {
    margin-top: 1rem;
}

.sidebar-menu ul {
    list-style: none;
    padding: 0;
    margin: 0;
}

.sidebar-menu li {
    display: flex;
    align-items: center;
    padding: 0.5rem 1rem;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

.sidebar-menu li:hover {
    background-color: #eaeaea;
}

.sidebar-menu i {
    margin-right: 1rem;
}

.sidebar-menu ul li {
    padding-left: 2rem;
}

.sidebar-menu ul li a {
    color: inherit;
    text-decoration: none;
}

.sidebar-menu ul li a:hover {
    color: #0c84e4;
}

.sidebar-menu ul ul {
    margin-top: 0.5rem;
    display: none;
}

.sidebar-menu ul li:hover > ul {
    display: block;
}

.sidebar-menu ul ul li {
    padding-left: 3rem;
}
</style>