async (page) => {
 const results=[];
 const routes=['/','/services','/packages','/projects','/about','/infrastructure','/contact','/request-proposal','/privacy-policy','/terms-and-conditions'];
 const names={'/':'landing','/services':'services','/packages':'packages','/projects':'projects'};
 for(const width of [1920,1280,768,390]) {
  await page.setViewportSize({width,height:1080});
  for(const path of (width===1920 || width===390 || width===768)?routes:Object.keys(names)) {
   const response=await page.goto('http://localhost:5148'+path);
   await page.evaluate(()=>document.fonts.ready);
   await page.waitForTimeout(200);
   for(let y=0;y<await page.evaluate(()=>document.documentElement.scrollHeight);y+=900) {
    await page.evaluate(y=>window.scrollTo(0,y),y); await page.waitForTimeout(80);
   }
   await page.evaluate(()=>window.scrollTo(0,0));
   await page.waitForTimeout(200);
   const state=await page.evaluate(()=>({title:document.title,lang:document.documentElement.lang,dir:document.documentElement.dir,width:innerWidth,scrollWidth:document.documentElement.scrollWidth,height:document.documentElement.scrollHeight,h1:document.querySelectorAll('h1').length,canonical:document.querySelector('link[rel=canonical]')?.href,description:document.querySelector('meta[name=description]')?.content,brokenImages:[...document.images].filter(i=>!i.complete||!i.naturalWidth).map(i=>i.src),placeholders:document.querySelectorAll('a[href="#"]').length,unlabelled:[...document.querySelectorAll('input,textarea,select')].filter(e=>!e.labels?.length&&!e.getAttribute('aria-label')).map(e=>e.id)}));
   results.push({path,viewport:width,status:response.status(),...state});
   if(names[path]) await page.screenshot({path:'output/playwright/'+names[path]+'-'+width+'.png',fullPage:true});
  }
 }
 const missing=await page.goto('http://localhost:5148/definitely-missing-public-page');
 results.push({path:'/definitely-missing-public-page',status:missing.status(),title:await page.title()});
 return results;
}
