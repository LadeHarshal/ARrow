import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {userAdmin, signup, signin} from "../controllers/userAdmin.js";


router.get("/", userAdmin)
router.post("/signup", signup)
router.post("/signin", session, signin)
// router.post("/signin", session, csrfProtect, signin)

export default router;